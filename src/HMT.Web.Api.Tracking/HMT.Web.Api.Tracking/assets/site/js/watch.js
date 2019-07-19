console.log("watch start");

// Change Service Domain Eg: http://localhost:85 or http://api.domain.com
var serviceDomain = location.origin;

// Change Service Address
var serviceApi = "/api/PrinterJob/Tracking";

// Full Address
var requestUri = serviceDomain + serviceApi;

/**
 * Perform an asynchronous HTTP (Ajax) request.
 * ref: https://api.jquery.com/jquery.ajax/
 * @param {string} uri string remote address
 */
function HttpRequest(uri) {

    // Request Method
    var method = "get";

    // Setup
    var resquest = $.ajax({
        type: method,
        url: uri
    });

    resquest
        // Data Done
        .done(function (data) {
            // Write Data
            BindData(data);
        })
        // Fail
        .fail(function (jqXHR) {

            // What is fail
            console.log(jqXHR.statusText);
        });
}

// Get Request
function GetRequest() {

    // Http Request
    HttpRequest(requestUri);
}

// Document Ready And Run
$(document).ready(function () {
    GetRequest();

    Timer();
});

// vue bind
// ref: https://vuejs.org/v2/guide/
var app = new Vue({
    el: '#app',
    data: {
        PrinterJobs: []
    }
});

/**
 * Bind Data
 * @param {List<PrinterJob>} data Printer Job List
 */
function BindData(data) {

    app.$data.PrinterJobs = data;
}

// Timer
// Ref : setTimeout
function Timer() {
    setTimeout(tick, 5000);
}

// Tick Timer
function tick() {
    console.log("Tick " + new Date());
    Timer();
    GetRequest();
}