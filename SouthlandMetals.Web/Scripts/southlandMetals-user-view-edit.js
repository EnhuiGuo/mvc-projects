$(document).ready(function () {

    $('#collapseAdministration').addClass("in");
    $('#collapseAdministration').attr("aria-expanded", "true");

    $('#administrationLink').addClass("category-current-link");
    $('#usersLink').addClass("current-link");

    var userName = "";
    var firstName = "";
    var lastName = "";
    var email = "";
    var isActive = true;

    $(document).on('click', '#deleteUserBtn', function () {
        event.preventDefault();
        $.confirm({
            text: 'Are you sure you want to delete this user?',
            dialogClass: "modal-confirm",
            confirmButton: "Yes",
            confirmButtonClass: 'btn btn-sm modal-confirm-btn',
            cancelButton: "No",
            cancelButtonClass: 'btn btn-sm btn-default',
            closeIcon: false,
            confirm: function (button) {
                $.ajax({
                    type: 'DELETE',
                    url: "/SouthlandMetals/Administration/User/Delete",
                    data: { 'userName': $('#userName').val() },
                    success: function (result) {
                        if (result.Success) {
                            window.location.href = "/SouthlandMetals/Administration/User/Index";
                        }
                        else {
                            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                    '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                    '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                        }
                    },
                    error: function (err) {
                        console.log('Error ' + err.responseText);
                    }
                });
            },
            cancel: function (button) {

            }
        });
    });

    $(document).on('click', '#resetPassword', function () {
        event.preventDefault();

        userName = $('#userName').val();
        firstName = $('#firstName').val();
        lastName = $('#lastName').val();
        email = $('#email').val();

        _ResetPassword();
    });

    $(document).on('click', '#userUpdateBtn', function () {
        event.preventDefault();

        if (!$('#userViewForm')[0].checkValidity()) {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                   '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                   '<strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');

            $('#userViewForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else if (roles.length < 1) {
            $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                 '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                 '<strong>Warning!</strong>&nbsp;You must select a role for this user!</div>');
        }
        else {

            var selectedVal = "";
            var selected = $("input[name='IsActive']:checked");
            if (selected.length > 0) {
                selectedVal = selected.val();
            }

            var model = {
                UserId: currentUserId,
                UserName: $('#userName').val(),
                FirstName: $('#firstName').val(),
                LastName: $('#lastName').val(),
                Email: $('#email').val(),
                Phone: $('#phone').val(),
                Department: $('#department').val(),
                Position: $('#position').val(),
                IsActive: selectedVal,
                ModifiedDate: new Date(),
                ModifiedBy: currentUser,
                Roles: roles,
            };
            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/User/Edit",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        window.location.href = "/SouthlandMetals/Administration/User/Index";
                    }
                    else {
                        $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                                '<strong>Warning!</strong>&nbsp;' + result.Message + '</div>');
                    }
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    $(document).on('click', '#cancelUpdateBtn', function () {
        window.history.back();
    });

    $('#cancelResetPasswordBtn').on('click', function () {
        $('#resetPasswordModal').modal('hide');
    });

    $('input:checkbox').on('change', function () {
        var tempRole = $(this);
        $.each(roles, function (i, role) {
            if (tempRole.val() == role.RoleName) {
                role.Selected = tempRole.prop("checked");
            }
        })
    });

    $(document).on('click', '#saveNewPasswordBtn', function () {
        event.preventDefault();

        if (!$('#userResetPasswordForm')[0].checkValidity()) {
            $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Please fill in all required fields!</div>');
            $('.errorAlert').show();

            $('#userResetPasswordForm input[required]').each(function () {
                if ($(this).val() === "") {
                    $(this).addClass("form-error");
                }
            });
        }
        else {
            var model = {
                UserName: $('#userName').val(),
                Password: $('#passwordInput').val()
            };
            $.ajax({
                type: "PUT",
                url: "/SouthlandMetals/Administration/User/ResetPassword",
                data: JSON.stringify(model),
                contentType: "application/json",
                dataType: "json",
                success: function (result) {
                    if (result.Success) {
                        $('.successAlert').append('<div><strong>Success!</strong>&nbsp;User password has been reset successfully.</div>');
                        $('.successAlert').show();
                        $('.successAlert').delay(3000).hide("fast");

                        $('#resetPasswordModal').modal('hide');
                    }
                    else {
                        $('.errorAlert').append('<div><strong>Warning!</strong>&nbsp;Unable to update user password!</div>');
                        $('.errorAlert').show();
                    }
                },
                error: function (err) {
                    console.log('Error ' + err.responseText);
                }
            });
        }
    });

    function _ResetPassword() {
        $.ajax({
            type: "GET",
            cache: false,
            url: "/SouthlandMetals/Administration/User/_ResetPassword",
            success: function (result) {

                $('#resetPasswordDiv').html('');
                $('#resetPasswordDiv').html(result);

                $('.successAlert').hide();
                $('.errorAlert').hide();

                $('#resetPasswordModal').modal('show');
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            },
            complete: function () {
                $('#resetUserName').val(userName);
                $('#resetFirstName').val(firstName);
                $('#resetLastName').val(lastName);
                $('#resetEmail').val(email);
                $('#resetIsActive').val(isActive);
            },
            error: function (err) {
                console.log('Error ' + err.responseText);
            }
        });
    };
});

  

