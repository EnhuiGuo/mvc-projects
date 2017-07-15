$(document).ready(function () {

    $('#operationsLink').addClass("category-current-link");
    $('#projectsLink').addClass("current-link");

    function statusFlag(data, type, row) {
        if (row.IsOpen) {
            return '<span>Open</span>';
        }
        else if (row.IsHold) {
            return '<span>On Hold</span>';
        }
        else if (row.IsCanceled) {
            return '<span>Canceled</span>';
        }
        else if (row.IsComplete) {
            return '<span>Complete</span>';
        }
    }

    var projectsTable = $('#projects').DataTable({
        dom: 'frt' +
             "<'row'<'col-sm-4'i><'col-sm-8'p>>",
        "autoWidth": false,
        "pageLength": 20,
        "lengthChange": false,
        "order": [[5, 'desc'], [1, 'asc']],
        "data": projects,
        "columns": [
        { "data": "ProjectId", "title": "ProjectId", "visible": false },
        { "data": "ProjectName", "title": "Name", "class": "center" },
        { "data": "CustomerName", "title": "Customer", "class": "center" },
        { "data": "FoundryName", "title": "Foundry", "class": "center" },
        { "data": "CreatedDate", "name": "CreatedDate", "title": "Created Date", "class": "center", render: dateFlag },
        { "data": "CreatedBy", "title": "Created By", "class": "center" },
        { "title": "Status", "class": "center", "render": statusFlag },
        { "data": "HoldExpirationDate", "name": "HoldExpirationDate", "title": "Expires", "visible": false, render: dateFlag },
        { "data": "CanceledDate", "name": "CanceledDate", "title": "Canceled Date", "visible": false, render: dateFlag },
        { "data": "CompletedDate", "name": "CompletedDate", "title": "Completed Date", "visible": false, render: dateFlag },
        { "title": "Hold Notes", "visible": false },
        { "title": "Cancel Notes", "visible": false },
        { "title": "View", "class": "center" }
        ],
        "columnDefs": [
             {
                 "targets": 10,
                 "title": "Hold Notes",
                 "width": "8%", "targets": 10,
                 "data": null,
                 "defaultContent":
                      "<span class='glyphicon glyphicon-file glyphicon-large' id='holdNotesBtn'></span>"
             },
             {
                 "targets": 11,
                 "title": "Cancel Notes",
                 "width": "8%", "targets": 11,
                 "data": null,
                 "defaultContent":
                      "<span class='glyphicon glyphicon-file glyphicon-large' id='cancelNotesBtn'></span>"
             },
            {
                "targets": 12,
                "title": "View",
                "width": "8%", "targets": 12,
                "data": null,
                "defaultContent":
                     "<span class='glyphicon glyphicon-info-sign glyphicon-large' id='viewBtn'></span>"
            }
        ]
    });

    $("input[name='Status']").change(function () {
        if (this.value == 'Open') {
            getOpenProjects();

            var taskColumn = projectsTable.column(6);
            taskColumn.visible(true);
            var holdColumn = projectsTable.column(7);
            holdColumn.visible(false);
            var completeColumn = projectsTable.column(8);
            completeColumn.visible(false);
            var canceledColumn = projectsTable.column(9);
            canceledColumn.visible(false);
            var holdNotesColumn = projectsTable.column(10);
            holdNotesColumn.visible(false);
            var cancelNotesColumn = projectsTable.column(11);
            cancelNotesColumn.visible(false);
        }
        else if (this.value == 'Hold') {
            getHoldProjects();

            var taskColumn = projectsTable.column(6);
            taskColumn.visible(true);
            var holdColumn = projectsTable.column(7);
            holdColumn.visible(true);
            var completeColumn = projectsTable.column(8);
            completeColumn.visible(false);
            var canceledColumn = projectsTable.column(9);
            canceledColumn.visible(false);
            var holdNotesColumn = projectsTable.column(10);
            holdNotesColumn.visible(true);
            var cancelNotesColumn = projectsTable.column(11);
            cancelNotesColumn.visible(false);
        }
        else if (this.value == 'Completed') {
            getCompleteProjects();

            var taskColumn = projectsTable.column(6);
            taskColumn.visible(false);
            var holdColumn = projectsTable.column(7);
            holdColumn.visible(false);
            var completeColumn = projectsTable.column(8);
            completeColumn.visible(true);
            var canceledColumn = projectsTable.column(9);
            canceledColumn.visible(false);
            var holdNotesColumn = projectsTable.column(10);
            holdNotesColumn.visible(false);
            var cancelNotesColumn = projectsTable.column(11);
            cancelNotesColumn.visible(false);
        }
        else if (this.value == 'Canceled') {
            getCanceledProjects();

            var taskColumn = projectsTable.column(6);
            taskColumn.visible(false);
            var holdColumn = projectsTable.column(7);
            holdColumn.visible(false);
            var completeColumn = projectsTable.column(8);
            completeColumn.visible(false);
            var canceledColumn = projectsTable.column(9);
            canceledColumn.visible(true);
            var holdNotesColumn = projectsTable.column(10);
            holdNotesColumn.visible(false);
            var cancelNotesColumn = projectsTable.column(11);
            cancelNotesColumn.visible(true);
        }
    });

    $('#projects tbody').on('click', '#holdNotesBtn', function () {
        var project = projectsTable.row($(this).parents('tr')).data();
        var childData = projectsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (project != null) {
            data = project;
        }
        else {
            data = childData;
        }
        _ViewProjectHoldNotes(data.ProjectId);
    });

    $('#projects tbody').on('click', '#cancelNotesBtn', function () {
        var project = projectsTable.row($(this).parents('tr')).data();
        var childData = projectsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (project != null) {
            data = project;
        }
        else {
            data = childData;
        }
        _ViewProjectCancelNotes(data.ProjectId);
    });

    $('#projects tbody').on('click', '#viewBtn', function () {
        var project = projectsTable.row($(this).parents('tr')).data();
        var childProjectData = projectsTable.row($(this).parents('tr').prev('tr')).data();
        var data;

        if (project != null) {
            data = project;
        }
        else {
            data = childProjectData;
        }
        window.open("/SouthlandMetals/Operations/Project/Summary?projectId=" + data.ProjectId, target = "_self");
    });
});

function getOpenProjects() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Project/GetOpenProjects",
        success: function (result) {
            $('#projects').DataTable().clear().draw();
            $('#projects').DataTable().rows.add(result.Projects); // Add new data
            $('#projects').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function getHoldProjects() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Project/GetHoldProjects",
        success: function (result) {
            $('#projects').DataTable().clear().draw();
            $('#projects').DataTable().rows.add(result.Projects); // Add new data
            $('#projects').DataTable().columns.adjust().draw();
        },
        error: function (req, err) {
            console.log('Error ' + err);
        }
    });
};

function getCompleteProjects() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Project/GetCompleteProjects",
        success: function (result) {
            $('#projects').DataTable().clear().draw();
            $('#projects').DataTable().rows.add(result.Projects); // Add new data
            $('#projects').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function getCanceledProjects() {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Project/GetCanceledProjects",
        success: function (result) {
            $('#projects').DataTable().clear().draw();
            $('#projects').DataTable().rows.add(result.Projects); // Add new data
            $('#projects').DataTable().columns.adjust().draw();
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _ViewProjectHoldNotes(projectId) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Project/_ViewProjectHoldNotes",
        data: { "projectId": projectId },
        contentType: "application/json",
        success: function (result) {

            $('#viewProjectHoldNotesDiv').html('');
            $('#viewProjectHoldNotesDiv').html(result);

            $('#viewProjectHoldNotesModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};

function _ViewProjectCancelNotes(projectId) {
    $.ajax({
        type: "GET",
        cache: false,
        url: "/SouthlandMetals/Operations/Project/_ViewProjectCancelNotes",
        data: { "projectId": projectId },
        contentType: "application/json",
        success: function (result) {

            $('#viewProjectCancelNotesDiv').html('');
            $('#viewProjectCancelNotesDiv').html(result);

            $('#viewProjectCancelNotesModal').modal('show');
        },
        error: function (err) {
            console.log('Error ' + err.responseText);
        }
    });
};