(function() {
    
    function PreviewController(scope, http, route) {
        scope.title = "Class preview loading...";
        scope.name = "";
        scope.code = "";

        http.get("/umbraco/codegen/preview/getpreview/" + route.id)
            .success(function(codeDto) {
                scope.name = codeDto.Name;
                scope.title = scope.name + " class preview";
                scope.code = codeDto.Code;
            });
    }

    function CodeMirrorDirective(assetsService) {
        function resizeMirror(mirror, jqElem) {
            var wrapper = $(mirror.getWrapperElement()),
                panel = wrapper.parents(".umb-panel-body"),
                pane = wrapper.parents(".umb-pane");
            pane.height(panel.height() - 60);
            mirror.setSize(pane.innerWidth()-20, pane.innerHeight());
            mirror.refresh();
        }

        return {
            link: function (scope, jqElem, attrs) {
                jqElem.hide();
                assetsService
                    .load([
                        "/umbraco_client/CodeMirror/js/lib/codemirror.js",
                        "/umbraco_client/CodeMirror/js/mode/razor/razor.js"
                    ])
                    .then(function() {
                        return assetsService.loadCss([
                            "/umbraco_client/CodeMirror/js/lib/codemirror.css"
                        ]);
                    })
                    .then(function() {
                        var mirror = CodeMirror.fromTextArea(jqElem.get(0), {
                            width: "100%",
                            height: "100%",
                            tabMode: "shift",
                            matchBrackets: true,
                            indentUnit: 4,
                            indentWithTabs: true,
                            enterMode: "keep",
                            lineWrapping: false,
                            lineNumbers: true
                        });

                        scope.$watch(function () {
                            return scope.$parent[attrs.ngModel];
                        }, function (newValue) {
                            mirror.setValue(newValue);
                        });

                        $(window).resize(function () {
                            resizeMirror(mirror, jqElem);
                        });
                        resizeMirror(mirror, jqElem);
                    });

            }
        }
    }

    angular.module("umbraco").controller("codegen.preview.controller", ["$scope", "$http", "$routeParams", PreviewController]);
    angular.module("umbraco").directive("cgCodemirror", ["assetsService", CodeMirrorDirective]);

}());