angular.module("myApp", [])
    .controller("PageCtrl",
        function($scope, $rootScope, $timeout, $interval) {
            $scope.schats = {
                activeChat: {},
                currentMessage: "",
                sendMessage: function() {

                    signalx.server.getMessageId("",
                        function(id) {
                            if (!$scope.schats.currentMessage) {
                                return;
                            }
                            signalx.server.sendMessage({
                                message: $scope.schats.currentMessage,
                                sender: "sam",
                                receiver: "bam",
                                sent: true,
                                messageId: id
                            });
                            $timeout(function() {
                                $scope.schats.currentMessage = "";
                            });
                        });
                },
                setActiveChat: function(chat) {
                    var arrayLength = $scope.schats.mainList.length;
                    for (var i = 0; i < arrayLength; i++) {
                        $scope.schats.mainList[i].active = false;
                    }
                    signalx.server.getUserMessages(chat.id,
                        function(response) {
                            chat.active = true;
                            chat.messages = response;
                            $scope.schats.activeChat = chat;
                        });
                }
            };
            $scope.schats.mainList = [];
            signalx.ready(function() {
                signalx.client.newMessage = function(message) {
                    $timeout(function() {
                        $scope.schats.activeChat.messages.push(message);
                    });
                };
                signalx.client.receiveNewConversation = function(data) {
                    $timeout(function() {
                        $scope.schats.mainList = $scope.schats.mainList.concat(data);
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