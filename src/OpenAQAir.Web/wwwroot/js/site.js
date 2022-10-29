// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function getCityData() {
    var keyword = $('#txtCity').val();
    var sortOrder = $("#slCitySortOrder option:selected").val()
    $.ajax({
        type: "POST",
        url: "/Home/PartialViewCity",
        data: { "keyword": keyword, "sortOrder": sortOrder },
        success: function (response) {
            $("#divCityDetails").html(response);
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });

}

function showForm(formName) {
    //-- Hide all forms
    $('.oForm').hide();
    $('.form-control').val('');
    
    if (formName == 'city') {
        $('.city-Form').show();
        $('#divCityDetails').html('');
    }
    else if (formName == 'country') {
        $('.country-Form').show();
        $('#divCountryDetails').html('');
    }
}

function getCountryData(pageNumber) {
    var keyword = $('#txtCountry').val();
    var sortOrder = $("#slCountrySortOrder option:selected").val()
    $("#divCountryDetails").html('');
    if (pageNumber == '') {
        //
    }
    $.ajax({
        type: "POST",
        url: "/Home/PartialViewCountry",
        data: { "keyword": keyword, "pageNumber": pageNumber, "sortOrder": sortOrder },
        success: function (response) {
            $("#divCountryDetails").html(response);
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });

}