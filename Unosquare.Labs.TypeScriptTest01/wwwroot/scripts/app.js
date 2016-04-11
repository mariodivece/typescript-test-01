var app;
(function (app) {
    'use strict';
    var TitleController = (function () {
        function TitleController($scope, $route, $location) {
            $scope.$on('$routeChangeSuccess', function () {
                $scope.pageTitle = $route.current.title;
            });
        }
        return TitleController;
    })();
    app.TitleController = TitleController;
    var ShoppingItem = (function () {
        function ShoppingItem() {
        }
        return ShoppingItem;
    })();
    app.ShoppingItem = ShoppingItem;
    var ShoppingItemsController = (function () {
        function ShoppingItemsController($scope, $http) {
            $scope.items = [];
            var $promise = $http.get('/api/shoppingitems/');
            $promise
                .success(function (items) {
                $scope.items = items;
            })
                .error(function (data) {
                alert('Error retrieving data');
            });
        }
        return ShoppingItemsController;
    })();
    app.ShoppingItemsController = ShoppingItemsController;
    var RouteConfiguration = (function () {
        function RouteConfiguration($routeProvider, $locationProvider) {
            var homeRoute;
            homeRoute = {
                title: 'Shopping Items Manager',
                templateUrl: '/views/home.html'
            };
            $routeProvider
                .when('/', homeRoute)
                .otherwise(homeRoute);
            $locationProvider.html5Mode(true);
        }
        return RouteConfiguration;
    })();
    app.RouteConfiguration = RouteConfiguration;
    angular.module('app', ['ngRoute'])
        .config(RouteConfiguration)
        .controller('TitleController', TitleController)
        .controller('ShoppingItemsController', ShoppingItemsController);
})(app || (app = {}));
