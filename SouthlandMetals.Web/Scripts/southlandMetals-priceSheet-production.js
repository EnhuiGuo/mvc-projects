$(document).ready(function () {

    $('#operationsLink').addClass("category-current-link");
    $('#analysisLink').addClass("current-link");

    var AddOnBucket = function (id, name, value, margin, waf) {
        var self = this;
        self.bucketId = ko.observable(id);
        self.bucketName = ko.observable(name);
        self.bucketValue = ko.observable(value);
        self.buckWaf = ko.observable(waf);
        self.bucketMargin = ko.observable(margin);
        self.bucketMarginText = ko.computed(function () {
            return self.bucketMargin() + '%';
        });

        self.bucketPNumber = ko.computed(function () {
            var data = self.bucketValue() / self.buckWaf();
            return data.toFixed(3);
        });
        self.bucketSell = ko.computed(function () {
            var data = self.bucketPNumber() * (1 + self.bucketMargin() / 100);
            return data.toFixed(3);
        });
    };

    var SurchargeBucket = function (id, name, value, margin) {
        var self = this;
        self.bucketId = ko.observable(id);
        self.bucketName = ko.observable(name);
        self.bucketValue = ko.observable(value);
        self.bucketMargin = ko.observable(margin);
        self.bucketMarginText = ko.computed(function () {
            return self.bucketMargin() + '%';
        });
        self.bucketSell = ko.computed(function () {
            var data = self.bucketValue() * (1 + self.bucketMargin() / 100);
            return data.toFixed(3);
        });
    };

    var DutyBucket = function (id, name, value, margin) {
        var self = this;
        self.bucketId = ko.observable(id);
        self.bucketName = ko.observable(name);
        self.bucketValue = ko.observable(value);
        self.bucketMargin = ko.observable(margin);
        self.bucketMarginText = ko.computed(function () {
            return self.bucketMargin() + '%';
        });
        self.bucketSell = ko.computed(function () {
            var data = self.bucketValue() * (1 + self.bucketMargin() / 100);
            return data.toFixed(3);
        });

        self.dutyValueText = ko.computed(function () {
            return self.bucketValue() + '%';
        });
        self.dutySellValueText = ko.computed(function () {
            return self.bucketSell() + '%';
        });
    };

    var CostPart = function (priceSheetPartId, projectPartId, number, weight, usage, raw, mach, addOnTotalPNumber, surchargeTotalValue, dutyTotalValue, fixtureCost, patternCost, currentPrice) {
        var self = this;
        self.priceSheetPartId = ko.observable(priceSheetPartId);
        self.projectPartId = ko.observable(projectPartId);
        self.costPartNumber = ko.observable(number);
        self.costPartWeight = ko.observable(weight);
        self.costAnnualUsage = ko.observable(usage);
        self.addOnTotalPNumber = ko.observable(addOnTotalPNumber);
        self.surchargeTotalValue = ko.observable(surchargeTotalValue);
        self.dutyTotalValue = ko.observable(dutyTotalValue);
        self.rawCost = ko.observable(raw);
        self.fixtureCost = ko.observable(fixtureCost);
        self.patternCost = ko.observable(patternCost);
        self.currentPrice = ko.observable(currentPrice);

        self.annualRawCost = ko.computed(function () {
            var annualRawCost = parseFloat(self.rawCost()) * removeNumberComma(self.costAnnualUsage());
            return parseFloat(annualRawCost).toFixed(3);
        });
        self.costPNumber = ko.computed(function () {
            var costPNumber = parseFloat(self.rawCost()) / parseFloat(self.costPartWeight());
            if (costPNumber == Infinity) {
                costPNumber = 0.00;
            }
            return parseFloat(costPNumber).toFixed(3);
        });
        self.machineCost = ko.observable(mach);
        self.fobCost = ko.computed(function () {
            var fobCost = parseFloat(self.rawCost()) + parseFloat(self.machineCost());
            return parseFloat(fobCost).toFixed(3);
        });
        self.costPartAddOn = ko.computed(function () {
            var costPartAddOn = parseFloat(self.costPartWeight()) * parseFloat(self.addOnTotalPNumber());
            return parseFloat(costPartAddOn).toFixed(3);
        });
        self.costPartSurcharge = ko.computed(function () {
            var costPartSurcharge = parseFloat(self.costPartWeight()) * parseFloat(self.surchargeTotalValue());
            return parseFloat(costPartSurcharge).toFixed(3);
        });
        self.costPartDuty = ko.computed(function () {
            var costPartDuty = (parseFloat(self.fobCost()) + parseFloat(self.costPartSurcharge())) * parseFloat(self.dutyTotalValue() / 100);
            return parseFloat(costPartDuty).toFixed(3);
        });
        self.cost = ko.computed(function () {
            var cost = parseFloat(self.fobCost()) + parseFloat(self.costPartSurcharge()) + parseFloat(self.costPartAddOn()) + parseFloat(self.costPartDuty());
            return parseFloat(cost).toFixed(3);
        });
        self.annualCost = ko.computed(function () {
            var annualCost = parseFloat(self.cost()) * removeNumberComma(self.costAnnualUsage());
            return parseFloat(annualCost).toFixed(3);
        });
    };

    var PricePart = function (priceSheetPartId, projectPartId, number, weight, usage, raw, machine, addOnTotalPNumber, surchargeTotalValue, dutyTotalValue, fixturePrice, patternPrice, currentPrice) {
        var self = this;
        self.priceSheetPartId = ko.observable(priceSheetPartId);
        self.projectPartId = ko.observable(projectPartId);
        self.pricePartNumber = ko.observable(number);
        self.pricePartWeight = ko.observable(weight);
        self.priceAnnualUsage = ko.observable(usage);
        self.rawPrice = ko.observable(raw);
        self.machinePrice = ko.observable(machine.toFixed(3));
        self.addOnTotalPNumber = ko.observable(addOnTotalPNumber);
        self.surchargeTotalValue = ko.observable(surchargeTotalValue);
        self.dutyTotalValue = ko.observable(dutyTotalValue);
        self.fixturePrice = ko.observable(fixturePrice.toFixed(3));
        self.patternPrice = ko.observable(patternPrice.toFixed(3));
        self.calculatedPrice = ko.observable();
        self.priceDifference = ko.observable();
        self.price = ko.observable();
        self.currentPrice = ko.observable(currentPrice);

        self.annualRawPrice = ko.computed(function () {
            var annualRawPrice = parseFloat(self.rawPrice()) * removeNumberComma(self.priceAnnualUsage());
            return parseFloat(annualRawPrice).toFixed(2);
        });
        self.pricePNumber = ko.computed(function () {
            var pricePNumber = parseFloat(self.rawPrice()) / parseFloat(self.pricePartWeight());
            if (pricePNumber == Infinity)
            {
                pricePNumber = 0.0;
            }
            return parseFloat(pricePNumber).toFixed(2);
        });
        self.fobPrice = ko.computed(function () {
            var fobPrice = parseFloat(self.rawPrice()) + parseFloat(self.machinePrice());
            return parseFloat(fobPrice).toFixed(2);
        });
        self.priceAddOn = ko.computed(function () {
            var priceAddOn = parseFloat(self.pricePartWeight()) * parseFloat(self.addOnTotalPNumber());
            return parseFloat(priceAddOn).toFixed(2);
        });
        self.priceSurcharge = ko.computed(function () {
            var priceSurcharge = parseFloat(self.pricePartWeight()) * parseFloat(self.surchargeTotalValue());
            return parseFloat(priceSurcharge).toFixed(2);
        });
        self.priceDuty = ko.computed(function () {
            var priceDuty = (parseFloat(self.fobPrice()) + parseFloat(self.priceSurcharge())) * parseFloat(self.dutyTotalValue() / 100);
            return parseFloat(priceDuty).toFixed(2);
        });
        self.calculatedPrice = ko.computed(function () {
            var price = parseFloat(self.fobPrice()) + parseFloat(self.priceSurcharge()) + parseFloat(self.priceAddOn()) + parseFloat(self.priceDuty());
            return parseFloat(price).toFixed(2);
        });
        self.priceDifference = ko.computed(function () {
            var price = parseFloat(self.calculatedPrice()) - parseFloat(self.currentPrice());
            return parseFloat(price).toFixed(2);
        });
        self.price = ko.computed(function () {
            var price = parseFloat(self.calculatedPrice());
            return parseFloat(price).toFixed(2);
        });
        self.annualPrice = ko.computed(function () {
            var annualPrice = parseFloat(self.price()) * removeNumberComma(self.priceAnnualUsage());
            return parseFloat(annualPrice).toFixed(2);
        });
    };

    var PriceSheetViewModel = function () {
        var self = this;

        self.projectMargin = ko.observable().extend({ addPercentageFormatted: 2 });
        self.priceSheetNumber = ko.observable();
        self.waf = ko.observable();

        self.projectMargin = ko.observable(priceSheet.ProjectMargin);
        self.priceSheetNumber = ko.observable(priceSheetNumber);
        self.waf = ko.observable(priceSheet.WAF);
        self.projectMarginText = ko.computed(function () {
            return self.projectMargin() + '%';
        });

        self.bucketType = ko.observable();
        self.bucketName = ko.observable();
        self.bucketValue = ko.observable();


        self.addOns = ko.observableArray([]);
        self.surcharges = ko.observableArray([]);
        self.duties = ko.observableArray([]);
        self.costParts = ko.observableArray([]);
        self.priceParts = ko.observableArray([]);

        self.addOnTotalPNumber = ko.pureComputed(function () {
            var total = 0;
            $.each(self.addOns(), function () {
                total += parseFloat(this.bucketPNumber());
            });
            return parseFloat(total).toFixed(3);
        });

        self.addOnTotalValue = ko.pureComputed(function () {
            var total = 0;
            $.each(self.addOns(), function () {
                total += parseFloat(this.bucketValue());
            });
            return parseFloat(total).toFixed(3);
        });
        self.addOnTotalSellValue = ko.pureComputed(function () {
            var total = 0;
            $.each(self.addOns(), function () {
                total += parseFloat(this.bucketSell());
            });
            return parseFloat(total).toFixed(3);
        });

        self.surchargeTotalValue = ko.pureComputed(function () {
            var total = 0;
            $.each(self.surcharges(), function () {
                total += parseFloat(this.bucketValue());
            });
            return parseFloat(total).toFixed(3);
        });
        self.surchargeTotalSellValue = ko.pureComputed(function () {
            var total = 0;
            $.each(self.surcharges(), function () {
                total += parseFloat(this.bucketSell());
            });
            return parseFloat(total).toFixed(3);
        });

        self.dutyTotalValue = ko.pureComputed(function () {
            var total = 0;
            $.each(self.duties(), function () {
                total += parseFloat(this.bucketValue());
            });
            return parseFloat(total).toFixed(2);
        });

        self.dutyTotalValueText = ko.pureComputed(function () {
            return self.dutyTotalValue() + '%';
        });

        self.dutyTotalSellValue = ko.pureComputed(function () {
            var total = 0;
            $.each(self.duties(), function () {
                total += parseFloat(this.bucketSell());
            });
            return parseFloat(total).toFixed(2);
        });

        self.dutyTotalSellValueText = ko.pureComputed(function () {
            return self.dutyTotalSellValue() + '%';
        });

        self.sumTotalAnnualCost = ko.pureComputed(function () {
            var total = 0;
            $.each(self.costParts(), function () {
                total += parseFloat(this.annualCost());
            });
            return parseFloat(total).toFixed(3);
        });

        self.sumTotalAnnualPrice = ko.pureComputed(function () {
            var total = 0;
            $.each(self.priceParts(), function (i, pricePart) {
                total += parseFloat(pricePart.annualPrice());
            });
            return parseFloat(total).toFixed(3);
        });

        self.toolingMargin = ko.pureComputed(function () {
            var totalInCost = 0, totalInPrice = 0;
            $.each(self.costParts(), function (i, costPart) {
                totalInCost += parseFloat(costPart.patternCost());
            });

            $.each(self.priceParts(), function (i, pricePart) {
                totalInPrice += parseFloat(pricePart.patternPrice());
            });
            return parseFloat(totalInPrice - totalInCost).toFixed(3);
        });


        self.fixtureMargin = ko.pureComputed(function () {
            var totalInCost = 0, totalInPrice = 0;
            $.each(self.costParts(), function (i, costPart) {
                totalInCost += parseFloat(costPart.fixtureCost());
            });

            $.each(self.priceParts(), function (i, pricePart) {
                totalInPrice += parseFloat(pricePart.fixturePrice());
            });
            return parseFloat(totalInPrice - totalInCost).toFixed(3);
        });

        self.overAllProfitMargin = ko.pureComputed(function () {
            var data = (self.sumTotalAnnualPrice() - self.sumTotalAnnualCost()) / self.sumTotalAnnualPrice();
            return data;
        });

        self.overAllProfitMarginText = ko.computed(function () {
            var text = parseFloat(self.overAllProfitMargin()).toFixed(2) * 100;
            return text.toFixed(2) + '%';
        });

        self.annualDollars = ko.pureComputed(function () {
            var total = 0;
            $.each(self.costParts(), function (i, costPart) {
                total += parseFloat(costPart.annualRawCost());
            });
            return parseInt(total);
        });

        self.annualMargin = ko.pureComputed(function () {
            var annualMargin = (parseFloat(self.sumTotalAnnualPrice()) - parseFloat(self.sumTotalAnnualCost())) / parseFloat(self.sumTotalAnnualCost());
            return annualMargin * 100;
        });

        self.annualMarginText = ko.pureComputed(function () {
            var annualMargin = self.annualMargin();
            return annualMargin.toFixed(2) + '%';
        });

        self.annualWeight = ko.pureComputed(function () {
            var total = 0;
            $.each(self.costParts(), function (i, costPart) {
                total += parseFloat(costPart.costPartWeight()) * parseFloat(costPart.costAnnualUsage());
            });
            return parseInt(total);
        });

        self.annualContainer = ko.pureComputed(function () {
            var annualContainer = parseFloat(self.annualWeight()) / parseFloat(self.waf());
            return parseFloat(annualContainer).toFixed(2);
        });

        self.dollarsContainer = ko.pureComputed(function () {
            var dollarsContainer = parseFloat(self.annualDollars()) / parseFloat(self.annualContainer());
            return parseInt(dollarsContainer);
        });

        self.insuranceFreight = ko.observable(2500);

        self.insurancePercentage = ko.pureComputed(function () {
            var insurancePercentage = (parseFloat(self.dollarsContainer()) + parseFloat(self.insuranceFreight())) * 1.1
            return parseInt(insurancePercentage);
        });

        self.insuranceDuty = ko.pureComputed(function () {
            var insuranceDuty = parseFloat(self.insurancePercentage()) * 1.03;
            return parseInt(insuranceDuty);
        });

        self.insuranceDivisor = ko.pureComputed(function () {
            var insuranceDivisor = parseFloat(self.insuranceDuty()) / 100;
            return parseInt(insuranceDivisor);
        });

        self.insurancePremium = ko.pureComputed(function () {
            var insurancePremium = parseFloat(self.insuranceDivisor()) * 0.49;
            return parseInt(insurancePremium);
        });





        self.addBucket = function () {
            if (self.projectMargin() === "") {
                $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                              '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                              '<strong>Warning!</strong>&nbsp;Please enter Project Margin!</div>');
            }
            else {
                if (self.bucketType() === "Add-On") {
                    var pNumber = self.bucketValue() / self.waf();
                    var profit = pNumber * (self.projectMargin() * 0.01);
                    var bucketSellValue = (parseFloat(pNumber.toFixed(3)) + parseFloat(profit)).toFixed(3);
                    self.addOns.push(new AddOnBucket(null, self.bucketName(), self.bucketValue(), self.projectMargin(), self.waf()));
                    clearAddBucket();
                }
                else if (self.bucketType() === "Surcharge") {
                    var profit = self.bucketValue() * (self.projectMargin() * 0.01);
                    var bucketSellValue = (parseFloat(self.bucketValue()) + parseFloat(profit)).toFixed(3);
                    self.surcharges.push(new SurchargeBucket(null, self.bucketName(), self.bucketValue(), self.projectMargin()));
                    clearAddBucket();
                }
                else if (self.bucketType() === "Duty") {
                    var profit = self.bucketValue() * (self.projectMargin() * 0.01);
                    var bucketSellValue = (parseFloat(self.bucketValue()) + parseFloat(profit)).toFixed(3);
                    self.duties.push(new DutyBucket(null, self.bucketName(), self.bucketValue(), self.projectMargin()));
                    clearAddBucket();
                }
                else {
                    $('#alertDiv').html('<div class="alert alert-danger alert-dismissable">' +
                                      '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                                      '<strong>Warning!</strong>&nbsp;Please select a Bucket Type!</div>');
                }
            }
        };

        self.savePriceSheet = function () {
            var priceDetailList = [];
            var costDetailList = [];
            var bucketList = [];

            $.each(self.addOns(), function (i, addOn) {
                if (addOn.bucketName() != "Total") {
                    var addOnDetail = {
                        "Value": addOn.bucketValue(),
                        "Margin": addOn.bucketMargin(),
                        "SellValue": addOn.bucketSell(),
                        "PNumber": addOn.bucketPNumber(),
                        "Name": addOn.bucketName(),
                        "PriceSheetId": priceSheetId,
                        "IsAddOn": true,
                    }
                    bucketList.push(addOnDetail);
                }
            });

            $.each(self.surcharges(), function (i, surcharge) {
                if (surcharge.bucketName() != "Total") {
                    var surchargeDetail = {
                        "Value": surcharge.bucketValue(),
                        "Margin": surcharge.bucketMargin(),
                        "SellValue": surcharge.bucketSell(),
                        "Name": surcharge.bucketName(),
                        "PriceSheetId": priceSheetId,
                        "IsSurcharge": true,
                    }
                    bucketList.push(surchargeDetail);
                }
            });

            $.each(self.duties(), function (i, duty) {
                if (duty.bucketName() != "Total") {
                    var dutyDetail = {
                        "Value": duty.bucketValue(),
                        "Margin": duty.bucketMargin(),
                        "SellValue": duty.bucketSell(),
                        "Name": duty.bucketName(),
                        "PriceSheetId": priceSheetId,
                        "IsDuty": true,
                    }
                    bucketList.push(dutyDetail);
                }
            });

            $.each(self.costParts(), function (i, costPart) {
                var costDetail = {
                    "PriceSheetPartId": costPart.priceSheetPartId(),
                    "ProjectPartId": costPart.projectPartId(),
                    "PartNumber": costPart.costPartNumber(),
                    "Weight": costPart.costPartWeight(),
                    "AnnualUsage": costPart.costAnnualUsage(),
                    "RawCost": costPart.rawCost(),
                    "AnnualRawCost": costPart.annualRawCost(),
                    "PNumber": costPart.costPNumber(),
                    "MachineCost": costPart.machineCost(),
                    "FOBCost": costPart.fobCost(),
                    "AddOn": costPart.costPartAddOn(),
                    "Surcharge": costPart.costPartSurcharge(),
                    "Duty": costPart.costPartDuty(),
                    "Cost": costPart.cost(),
                    "AnnualCost": costPart.annualCost(),
                    "PriceSheetId": priceSheetId,
                    "FixtureCost": costPart.fixtureCost(),
                    "PatternCost": costPart.patternCost()
                }
                costDetailList.push(costDetail);
            });

            $.each(self.priceParts(), function (i, pricePart) {
                var priceDetail = {
                    "PriceSheetPartId": pricePart.priceSheetPartId(),
                    "ProjectPartId": pricePart.projectPartId(),
                    "PartNumber": pricePart.pricePartNumber(),
                    "Weight": pricePart.pricePartWeight(),
                    "AnnualUsage": pricePart.priceAnnualUsage(),
                    "RawPrice": pricePart.rawPrice(),
                    "AnnualRawPrice": pricePart.annualRawPrice(),
                    "PNumber": pricePart.pricePNumber(),
                    "MachinePrice": pricePart.machinePrice(),
                    "FOBPrice": pricePart.fobPrice(),
                    "AddOn": pricePart.priceAddOn(),
                    "Surcharge": pricePart.priceSurcharge(),
                    "Duty": pricePart.priceDuty(),
                    "Price": pricePart.price(),
                    "AnnualPrice": pricePart.annualPrice(),
                    "PriceSheetId": priceSheetId,
                    "FixturePrice": pricePart.fixturePrice(),
                    "PatternPrice": pricePart.patternPrice(),
                    "PriceDifference": pricePart.priceDifference()
                }
                priceDetailList.push(priceDetail);
            });

            var priceSheetModel = {
                PriceSheetId: priceSheetId,
                Number: self.priceSheetNumber(),
                RfqId: rfqId,
                RfqNumber: rfqNumber,
                ProjectMargin: self.projectMargin(),
                WAF: parseInt(self.waf()),
                AnnualDollars: parseInt(self.annualDollars()),
                AnnualMargin: parseFloat(self.annualMargin()).toFixed(4),
                AnnualWeight: parseInt(self.annualWeight()),
                AnnualContainer: parseInt(self.annualContainer()),
                DollarContainer: parseInt(self.dollarsContainer()),
                InsuranceFreight: parseInt(self.insuranceFreight()),
                InsurancePercentage: parseInt(self.insurancePercentage()),
                InsuranceDuty: parseInt(self.insuranceDuty()),
                InsuranceDivisor: parseInt(self.insuranceDivisor()),
                InsurancePremium: parseInt(self.insurancePremium()),
                ToolingMargin: parseFloat(self.toolingMargin()),
                FixtureMargin: parseFloat(self.fixtureMargin()),
                PriceDetailList: priceDetailList,
                CostDetailList: costDetailList,
                BucketList: bucketList
            };

            $.ajax({
                type: 'POST',
                url: "/SouthlandMetals/Operations/Pricing/CreateProductionPriceSheet",
                dataType: "json",
                data: { "priceSheet": priceSheetModel },
                success: function (result) {
                    if (result.Success) {

                        $.confirm({
                            text: 'Create Success, Do you want print this Price Sheet?',
                            dialogClass: "modal-confirm",
                            confirmButton: "Yes",
                            confirmButtonClass: 'btn btn-sm',
                            cancelButton: "No",
                            cancelButtonClass: 'btn btn-sm btn-default',
                            closeIcon: false,
                            confirm: function (button) {
                                window.location.href = '/SouthlandMetals/Operations/Report/PriceSheetReport?priceSheetId=' + result.ReferenceId + '';
                            },
                            cancel: function (button) {
                                window.location.href = '/SouthlandMetals/Operations/Pricing/Index';
                            }
                        });
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
        };

        $(document).on('click', '#cancelSavePricingBtn', function () {
            window.history.back();
        });

        function clearAddBucket() {
            self.bucketName("");
            self.bucketValue("");
        };

        self.removeAddOnBucket = function (bucket) {
            self.addOns.remove(bucket);
        };

        self.removeSurchargeBucket = function (bucket) {
            self.surcharges.remove(bucket);
        };

        self.removeDutyBucket = function (bucket) {
            self.duties.remove(bucket);
        };

        self.loadAddOns = function () {
            $.each(priceSheet.BucketList, function (n, bucket) {
                if (bucket.IsAddOn)
                    self.addOns.push(new AddOnBucket(bucket.PriceSheetBucketId, bucket.Name, bucket.Value, bucket.Margin, self.waf()))
            });
        }

        self.loadSurcharges = function () {
            $.each(priceSheet.BucketList, function (n, bucket) {
                if (bucket.IsSurcharge)
                    self.surcharges.push(new SurchargeBucket(bucket.PriceSheetBucketId, bucket.Name, bucket.Value, bucket.Margin))
            });
        }

        self.loadDutys = function () {
            $.each(priceSheet.BucketList, function (n, bucket) {
                if (bucket.IsDuty)
                    self.duties.push(new DutyBucket(bucket.PriceSheetBucketId, bucket.Name, bucket.Value, bucket.Margin))
            });
        }

        self.loadCostParts = function () {
            $.each(priceSheet.CostDetailList, function (n, part) {
                self.costParts.push(new CostPart(part.PriceSheetPartId, part.ProjectPartId, part.PartNumber, part.Weight, part.AnnualUsage, part.RawCost, part.MachineCost, self.addOnTotalPNumber(), self.surchargeTotalValue(), self.dutyTotalValue(), part.FixtureCost, part.PatternCost, part.Price))
            });
        }

        self.loadCurrentPrice = function () {
            $.each(self.costParts(), function (n, part) {
                var existingPart = priceSheet.PriceDetailList.filter(function (n) {
                    return n.ProjectPartId === part.projectPartId();
                });

                part.currentPrice(existingPart[0].Price);
            });
        };

        self.loadPriceParts = function () {
            $.each(self.costParts(), function (n, part) {
                self.priceParts.push(new PricePart(
                    part.priceSheetPartId(), part.projectPartId(), part.costPartNumber(), part.costPartWeight(), part.costAnnualUsage(), part.rawCost() * (1 + self.projectMargin() / 100),
                    part.machineCost() * (1 + self.projectMargin() / 100), self.addOnTotalPNumber(), self.surchargeTotalValue(), self.dutyTotalValue(),
                    part.fixtureCost() * (1 + self.projectMargin() / 100), part.patternCost() * (1 + self.projectMargin() / 100),
                    part.currentPrice()
                    ));
            });
        };

        self.loadAddOns();
        self.loadSurcharges();
        self.loadDutys();
        self.loadCostParts();
        self.loadCurrentPrice();
        self.loadPriceParts();

        self.waf.subscribe(function () {
            self.checkAddOnWaf(self.waf());
        });

        self.checkAddOnWaf = function (value) {
            $.each(self.addOns(), function (n, addOn) {
                addOn.buckWaf(value);
            });
        };

        self.addOnTotalPNumber.subscribe(function () {
            self.checkCostPNumber(self.addOnTotalPNumber());
            self.checkPricePNumber(self.addOnTotalPNumber());
        });

        self.surchargeTotalValue.subscribe(function () {
            self.checkCostSurcharge(self.surchargeTotalValue());
            self.checkPriceSurcharge(self.surchargeTotalValue());
        });

        self.dutyTotalValue.subscribe(function () {
            self.checkCostDuty(self.dutyTotalValue());
            self.checkPriceDuty(self.dutyTotalValue());
        })

        self.checkCostPNumber = function (pNumber) {
            $.each(self.costParts(), function (n, costPart) {
                costPart.addOnTotalPNumber(pNumber);
            });
        }

        self.checkCostSurcharge = function (surcharge) {
            $.each(self.costParts(), function (n, costPart) {
                costPart.surchargeTotalValue(surcharge);
            });
        }

        self.checkCostDuty = function (duty) {
            $.each(self.costParts(), function (n, costPart) {
                costPart.dutyTotalValue(duty);
            });
        }

        self.checkPricePNumber = function (pNumber) {
            $.each(self.priceParts(), function (n, pricePart) {
                pricePart.addOnTotalPNumber(pNumber);
            });
        }

        self.checkPriceSurcharge = function (surcharge) {
            $.each(self.priceParts(), function (n, pricePart) {
                pricePart.surchargeTotalValue(surcharge);
            });
        }

        self.checkPriceDuty = function (duty) {
            $.each(self.priceParts(), function (n, pricePart) {
                pricePart.dutyTotalValue(duty);
            });
        }

        $.each(self.costParts(), function (i, costPart) {
            costPart.costPartWeight.subscribe(function () {
                self.checkPriceWeight(costPart.costPartWeight(), costPart.costPartNumber())
            });
        });

        $.each(self.costParts(), function (i, costPart) {
            costPart.costAnnualUsage.subscribe(function () {
                self.checkPriceAnnualUsage(costPart.costAnnualUsage(), costPart.costPartNumber())
            });
        });

        $.each(self.costParts(), function (i, costPart) {
            costPart.rawCost.subscribe(function () {
                self.checkPriceRaw(costPart.rawCost(), costPart.costPartNumber());
            });
        });

        $.each(self.costParts(), function (i, costPart) {
            costPart.machineCost.subscribe(function () {
                self.checkPriceMachine(costPart.machineCost(), costPart.costPartNumber());
            });
        });

        self.checkPriceWeight = function (costPartWeight, partNumber) {
            $.each(self.priceParts(), function (n, pricePart) {
                if (pricePart.pricePartNumber() == partNumber) {
                    pricePart.pricePartWeight(costPartWeight);
                }
            });
        }

        self.checkPriceAnnualUsage = function (costAnnualUsage, partNumber) {
            $.each(self.priceParts(), function (n, pricePart) {
                if (pricePart.pricePartNumber() == partNumber) {
                    pricePart.priceAnnualUsage(costAnnualUsage);
                }
            });
        }

        self.checkPriceRaw = function (rawPrice, partNumber) {
            $.each(self.priceParts(), function (n, pricePart) {
                if (pricePart.pricePartNumber() == partNumber) {
                    pricePart.rawPrice(parseFloat(rawPrice) * (1 + parseFloat(self.projectMargin()) / 100));
                }
            });
        }

        self.checkPriceMachine = function (machinePrice, partNumber) {
            $.each(self.priceParts(), function (n, pricePart) {
                if (pricePart.pricePartNumber() == partNumber) {
                    pricePart.machinePrice(parseFloat(machinePrice) * (1 + parseFloat(self.projectMargin()) / 100));
                }
            });
        }

        self.checkPriceMargin = function () {
            $.each(self.costParts(), function (i, costPart) {
                $.each(self.priceParts(), function (n, pricePart) {
                    if (pricePart.pricePartNumber() == costPart.costPartNumber()) {
                        pricePart.rawPrice((parseFloat(costPart.rawCost()) * (1 + parseFloat(self.projectMargin()) / 100)).toFixed(3));
                        pricePart.machinePrice((parseFloat(costPart.machineCost()) * (1 + parseFloat(self.projectMargin()) / 100)).toFixed(3));
                        pricePart.fixturePrice((parseFloat(costPart.fixtureCost()) * (1 + parseFloat(self.projectMargin()) / 100)).toFixed(3));
                        pricePart.patternPrice((parseFloat(costPart.patternCost()) * (1 + parseFloat(self.projectMargin()) / 100)).toFixed(3));
                    }
                });
            });
        }

        $.each(self.costParts(), function (i, costPart) {
            costPart.fixtureCost.subscribe(function () {
                self.checkFixturePrice(costPart.fixtureCost(), costPart.costPartNumber());
            });
        });

        $.each(self.costParts(), function (i, costPart) {
            costPart.patternCost.subscribe(function () {
                self.checkPatternPrice(costPart.patternCost(), costPart.costPartNumber());
            });
        });


        self.checkFixturePrice = function (fixtureCost, partNumber) {
            $.each(self.priceParts(), function (n, pricePart) {
                if (pricePart.pricePartNumber() == partNumber) {
                    pricePart.fixturePrice(parseFloat(fixtureCost) * (1 + parseFloat(self.projectMargin()) / 100));
                }
            });
        }

        self.checkPatternPrice = function (patternCost, partNumber) {
            $.each(self.priceParts(), function (n, pricePart) {
                if (pricePart.pricePartNumber() == partNumber) {
                    pricePart.patternPrice(parseFloat(patternCost) * (1 + parseFloat(self.projectMargin()) / 100));
                }
            });
        }

        self.projectMargin.subscribe(function () {
            self.checkPriceMargin();
        });

        self.calculateAddOnMargin = function (projectMargin) {
            $.each(self.addOns(), function (n, addOn) {
                addOn.bucketMargin(projectMargin);
            });
        };

        self.calculateSurchargeMargin = function (projectMargin) {
            $.each(self.surcharges(), function (n, surcharge) {
                surcharge.bucketMargin(projectMargin);
            });
        };

        self.calculateDutyMargin = function (projectMargin) {
            $.each(self.duties(), function (n, duty) {
                duty.bucketMargin(projectMargin);
            });
        };

        $('#bucketPNumber, #bucketName, #bucketValue').keyup(function (e) {
            if (e.keyCode == 13) {
                $(this).trigger("enterKey");
            }
        });

        $(document).bind('enterKey', '#bucketPNumber, #bucketName, #bucketValue', function () {
            self.addBucket();
            $('#bucketName').val('');
            $('#bucketValue').val('');
            $('#bucketPNumber').val('');
            $('#bucketName').focus();
        });
    };


    var pm = new PriceSheetViewModel();
    ko.applyBindings(pm);
});