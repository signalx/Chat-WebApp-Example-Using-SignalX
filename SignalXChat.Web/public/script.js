angular.module("myApp", [])
    .controller("PageCtrl",
        function($scope, $rootScope, $timeout, $interval) {
            $scope.schats = {
                activeChat: {},
                currentMessage: "",
                sendMessage: function() {
                    if (!$scope.schats.currentMessage) {
                        return;
                    }
                    signalx.server.sendMessage({
                        message: $scope.schats.currentMessage,
                        sent: true
                    });

                    $scope.schats.currentMessage = "";
                },
                setActiveChat: function(chat) {
                    var arrayLength = $scope.schats.mainList.length;
                    for (var i = 0; i < arrayLength; i++) {
                        $scope.schats.mainList[i].active = false;
                    }
                    chat.active = true;
                    $scope.schats.activeChat = chat;
                }
            };

            signalx.ready(function() {
                signalx.client.newMessage = function(message) {
                    $timeout(function() {
                        $scope.schats.activeChat.messages.push(message);
                    });
                };
                signalx.client.receiveNewConversation = function(data) {
                    $timeout(function() {
                        $scope.schats.mainList = data;
                    });
                };
                signalx.server.getChats("", "receiveNewConversation");
            });
        });

angular.module("myApp").directive("myEnter",
    function() {
        return function(scope, element, attrs) {
            element.bind("keydown keypress",
                function(event) {
                    if (event.which === 13) {
                        scope.$apply(function() {
                            scope.$eval(attrs.myEnter);
                        });

                        event.preventDefault();
                    }
                });
        };
    });
angular.element(function() {
    angular.bootstrap(document, ["myApp"]);
});