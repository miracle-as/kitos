﻿(function (ng, app) {
    app.config(['$stateProvider', function ($stateProvider) {
        $stateProvider.state('it-project.edit.status-project', {
            url: '/status-project',
            templateUrl: 'partials/it-project/tab-status-project.html',
            controller: 'project.EditStatusProjectCtrl',
            resolve: {
               
                //returns a map with those users who have a role in this project.
                //the names of the roles is saved in user.roleNames
                usersWithRoles: ['$http', '$stateParams', function ($http, $stateParams) {

                    //get the rights of the projects
                    return $http.get("api/itprojectright/" + $stateParams.id)
                        .then(function (rightResult) {
                            var rights = rightResult.data.response;

                            //get the role names
                            return $http.get("api/itprojectrole/")
                                .then(function (roleResult) {
                                    var roles = roleResult.data.response;

                                    //the resulting map
                                    var users = {};
                                    _.each(rights, function (right) {

                                        //use the user from the map if possible
                                        var user = users[right.userId] || right.user;
                                        
                                        var role = _.findWhere(roles, { id: right.roleId });

                                        var roleNames = user.roleNames || [];
                                        roleNames.push(role.name);
                                        user.roleNames = roleNames;
                                        
                                        users[right.userId] = user;
                                    });
                                    
                                    return users;

                                });

                        });
                }]
                
            }
        });
    }]);

    app.controller('project.EditStatusProjectCtrl',
    ['$scope', '$http', 'notify', '$modal', 'itProject', 'usersWithRoles',
        function ($scope, $http, notify, $modal, itProject, usersWithRoles) {
            $scope.project = itProject;
            $scope.project.updateUrl = "api/itproject/" + itProject.id;

            //Setup phases
            $scope.project.phases = [itProject.phase1, itProject.phase2, itProject.phase3, itProject.phase4, itProject.phase5];
            var prevPhase = null;
            _.each($scope.project.phases, function (phase) {
                phase.updateUrl = "api/activity/" + phase.id;
                phase.prevPhase = prevPhase;
                prevPhase = phase;
            });
            
            //Returns a phase (activity) given an id
            function findPhase(id) {
                return _.findWhere($scope.project.phases, { id: id });
            }
            
            //All activities - both activities ("opgaver") and milestones
            $scope.milestonesActivities = [];
            
            function addMilestoneActivity(activity) {
                activity.show = true;

                activity.updatePhase = function(id) {
                    activity.phase = _.findWhere($scope.project.phases, { id: activity.associatedActivityId });
                };

                activity.updatePhase();

                activity.updateUser = function() {
                    if (activity.associatedUserId) {
                        activity.associatedUser = _.findWhere(usersWithRoles, { id: activity.associatedUserId });
                    }
                };

                activity.updateUser();

                activity.edit = function() {
                    return editActivity(activity);
                };

                activity.delete = function() {

                    var msg = notify.addInfoMessage("Sletter... ");
                    $http.delete(activity.updateUrl).success(function() {

                        activity.show = false;

                        msg.toSuccessMessage("Slettet!");

                    }).error(function() {

                        msg.toErrorMessage("Fejl! Kunne ikke slette!");
                    });

                };

                $scope.milestonesActivities.push(activity);

                return activity;
            }

            //Add a taskActivity ("opgaver")
            function addMilestone(milestone) {
                milestone.isMilestone = true;
                milestone.updateUrl = "api/state/" + milestone.id;

                autoSaveTrafficLight(milestone.updateUrl, "status", function() {
                    return milestone.status;
                });

                return addMilestoneActivity(milestone);
            }

            //Add a milestoneState ("milepæle")
            function addTask(task) {
                task.isTask = true;
                task.updateUrl = "api/activity/" + task.id;

                return addMilestoneActivity(task);
            }

            _.each(itProject.taskActivities, addTask);
            _.each(itProject.milestoneStates, addMilestone);

            function patch(url, field, value) {
                var payload = {};
                payload[field] = value;

                return $http({
                    method: 'PATCH',
                    url: url,
                    data: payload
                });
            }

            $scope.updateSelectedPhase = function (phase) {
                patch($scope.project.updateUrl, "currentPhaseId", phase.id)
                    .success(function (result) {
                        notify.addSuccessMessage("Feltet er opdateret");
                        $scope.project.currentPhaseId = phase.id;
                    })
                    .error(function () {
                        notify.addErrorMessage("Fejl!");
                    });
            };


            $scope.updatePhaseDate = function(phase) {
                //Update start date of the current phase
                patch(phase.updateUrl, "startDate", phase.startDate)
                    .success(function (result) {
                        //Also update end date of the previous phase
                        patch(phase.prevPhase.updateUrl, "endDate", phase.startDate).success(function () {
                            notify.addSuccessMessage("Feltet er opdateret");
                        }).error(function () {
                            notify.addErrorMessage("Fejl!");
                        });

                    }).error(function () {
                        notify.addErrorMessage("Fejl!");
                    });
            };

            $scope.updateStatusDate = function() {
                patch($scope.project.updateUrl, "statusDate", $scope.project.statusDate)
                    .success(function () {
                        notify.addSuccessMessage("Feltet er opdateret");
                    }).error(function () {
                        notify.addErrorMessage("Fejl!");
                    });
            };
            
            function autoSaveTrafficLight(url, field, watchExp) {
                $scope.$watch(watchExp, function(newVal, oldVal) {

                    if (angular.isUndefined(newVal) || newVal == null || newVal == oldVal) return;

                    var msg = notify.addInfoMessage("Gemmer...", false);
                    patch(url, field, newVal).success(function(result) {
                        msg.toSuccessMessage("Feltet er opdateret");
                    }).error(function() {
                        msg.toErrorMessage("Fejl!");
                    });

                });
            }

            autoSaveTrafficLight($scope.project.updateUrl, "statusProject", function() {
                return $scope.project.statusProject;
            });
            
            $scope.addMilestone = function() {
                $http.post("api/state", { milestoneForProjectId: itProject.id }).success(function(result) {
                    var activity = result.response;

                    addMilestone(activity);
                    editActivity(activity);

                });
            };
            
            $scope.addTask = function () {
                $http.post("api/activity", { taskForProjectId: itProject.id }).success(function (result) {
                    var activity = result.response;

                    addTask(activity);
                    editActivity(activity);

                });
            };

            function editActivity(activity) {
                var modal = $modal.open({
                    templateUrl: 'partials/it-project/modal-milestone-task-edit.html',
                    controller: ['$scope', '$modalInstance', function ($modalScope, $modalInstance) {

                        $modalScope.activity = activity;

                        $modalScope.updateDate = function(field) {
                            patch(activity.updateUrl, field, activity[field]).success(function() {
                                notify.addSuccessMessage("Feltet er opdateret");
                            }).error(function() {
                                notify.addErrorMessage("Fejl!");
                            });
                        };

                        $modalScope.phases = $scope.project.phases;

                        $modalScope.usersWithRoles = _.values(usersWithRoles);

                        $modalScope.updateUserName = $modalScope.activity.updateUser;

                        $modalScope.updatePhase = $modalScope.activity.updatePhase;
                    }]
                });
            }
            


        }]);
})(angular, app);