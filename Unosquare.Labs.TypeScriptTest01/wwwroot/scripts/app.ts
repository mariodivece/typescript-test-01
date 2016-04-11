module app {
    'use strict';

    export interface IExtendedRoute extends ng.route.IRoute {
        title: string;
    }

    export interface ITitleControllerScope extends ng.IScope {
        pageTitle: string;
    }

    export class TitleController {
        constructor($scope: ITitleControllerScope, $route: ng.route.IRouteService, $location: ng.ILocationService) {
            $scope.$on('$routeChangeSuccess', function () {
                $scope.pageTitle = (<any>$route.current).title;
            });
        }
    }

    export class ShoppingItem {
        Name: string;
        Description: string;
        Stock: number;
        Price: number;
        LastModified: Date;
    }

    export interface ShoppingItemsControllerScope extends ng.IScope {
        items: ShoppingItem[];
    }

    export class ShoppingItemsController {
        constructor($scope: ShoppingItemsControllerScope, $http: ng.IHttpService) {
            $scope.items = [];
            var $promise = $http.get<ShoppingItem[]>('/api/shoppingitems/');
            $promise
                .success(function (items) {
                    $scope.items = items;
                })
                .error(function (data) {
                    alert('Error retrieving data');
                });

        }
    }

    export class RouteConfiguration {
        constructor($routeProvider: ng.route.IRouteProvider, $locationProvider: ng.ILocationProvider) {

            var homeRoute: IExtendedRoute;
            homeRoute = {
                title: 'Shopping Items Manager',
                templateUrl: '/views/home.html'
            };


            $routeProvider
                .when('/', homeRoute)
                .otherwise(homeRoute);

            $locationProvider.html5Mode(true);
        }
    }

    angular.module('app', ['ngRoute'])
        .config(RouteConfiguration)
        .controller('TitleController', TitleController)
        .controller('ShoppingItemsController', ShoppingItemsController);

}