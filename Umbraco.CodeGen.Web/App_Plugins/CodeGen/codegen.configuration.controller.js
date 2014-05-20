(function() {

    function ConfigurationController(scope, http, q, notificationsService) {
        var root = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath,
            promises = [];

        function findValidDataTypes() {
            if (!scope.dataTypes) {
                return [];
            }
            return $.grep(scope.dataTypes, function (dt) {
                return $.grep(scope.config.TypeMappings.Items, function(tm) {
                        return tm.DataTypeId == dt.alias;
                    }).length == 0 &&
                    dt.alias != scope.newTypeMapping.DataTypeId;
            });
        }

        function resetNewTypeMapping() {
            scope.newTypeMapping = {
                DataTypeId: "",
                Type: "",
                Description: ""
            };
        }

        scope.config = null;
        scope.factories = [];

        scope.contentTypeTemplate = Umbraco.Sys.ServerVariables.umbracoSettings.appPluginsPath + "/CodeGen/codegen.configuration.contentType.html";

        resetNewTypeMapping();

        promises.push(http.get(root + "/CodeGen/Configuration/Get"));
        promises.push(http.get(root + "/CodeGen/Configuration/GetFactories"));
        promises.push(http.get(root + "/CodeGen/Configuration/GetDataTypes"));

        q.all(promises).then(function(results) {
            scope.config = results[0].data;
            scope.factories = results[1].data;
            scope.dataTypes = results[2].data;
            scope.validDataTypes = findValidDataTypes();

            scope.contentTypes = [
                {
                    name: "Document Types",
                    type: scope.config.DocumentTypes
                }, {
                    name: "Media Types",
                    type: scope.config.MediaTypes
                }
            ];
        });

        scope.setFactory = function(factory) {
            scope.config.GeneratorFactory = factory.GeneratorFactory;
            scope.config.ParserFactory = factory.ParserFactory;
        }

        scope.validDataTypes = [];

        scope.evaluateDataTypes = function() {
            scope.validDataTypes = findValidDataTypes();
        }

        scope.setDataType = function(typeMapping, dataType) {
            typeMapping.DataTypeId = dataType.alias;
            scope.evaluateDataTypes();
        }

        scope.addDataType = function() {
            scope.config.TypeMappings.Items.push(scope.newTypeMapping);
            resetNewTypeMapping();
            scope.evaluateDataTypes();
        }

        scope.removeDataType = function($index) {
            scope.config.TypeMappings.Items.splice($index, 1);
            scope.evaluateDataTypes();
        }

        scope.propose = function(input) {
            var promise = http.get(root + "/CodeGen/Configuration/GetTypeProposal", { params: { input: input } });
            scope.$apply();
            return promise;
        }

        scope.save = function() {
            http.post(root + "/CodeGen/Configuration/Post", scope.config)
                .success(function() {
                    notificationsService.showNotification({
                        type: 0,
                        header: "Configuration saved",
                        message: "Configuration was saved and reloaded."
                    });
                })
                .error(function() {
                    notificationsService.showNotification({
                        type: 2,
                        header: "Save failed",
                        message: data
                    });
                });
        }
    }

    function Typeahead() {
        return {
            restrict: "A",
            //replace: false,
            require: "ngModel",
            link: function (scope, element, attrs, ngModel) {
                var dropDown = $("<ul class=\"dropdown-menu\"></ul>"),
                    typeahead = scope.$eval(attrs.codegenTypeahead),
                    model = ngModel;
                element.after(dropDown);
                element.parent().addClass("dropdown");
                element.keyup(function (e) {
                    console.log("calling");
                    typeahead(element.val())
                        .success(function(values) {
                            console.log("result");
                            dropDown.html($.map(values, function (val) {
                                return "<li><a href=\"javascript:void(null);\">" + val + "</a></li>";
                            }));
                            dropDown.children("li").click(function () {
                                var val = $(this).children("a").text();
                                ngModel.$setViewValue(val);
                                element.val(val);
                                element.dropdown("toggle");
                            });
                            element.dropdown("toggle");
                        })
                        .error(function() {
                        });
                });
            }
        }
    }

    angular.module("umbraco").directive("codegenTypeahead", Typeahead);

    angular.module("umbraco").controller("codegen.configuration.controller", [
        "$scope",
        "$http",
        "$q",
        "notificationsService",
        ConfigurationController
    ]);

}());