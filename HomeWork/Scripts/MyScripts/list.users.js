﻿var urlDeleteUser = "";
var urlDetails = "";
var userID = "";
var urlEditPost = "";

$('#UserListBox').on('click', 'a[href = "#"]', function () {
    var val = $(this).attr('UserID');

    $("#user_content").load(urlDetails, { id: val });
});

$('#UserListBox').on('click', '.list-group-item', function () {
    $("#select-validation").empty();
  //  userID = $(this).children('span').attr('UserID');
    userID = $("#Details").attr('UserID');
    $('.nav li').removeClass('active');
    $(this).addClass('active');
});


$('#EditUser').click(function () {
    if (!userID == "") {
        $.get(urlEditGet, { id: userID }, function (result) {
            $("#MainContainer").html(result);
        });
    }
    else {
        $("#select-validation").text("Please select user!");
    }
});

$('#DelUser').click(function (e) {
    $.get(urlDeleteUser, { id: userID }, function () {
        location.reload();
    });

    e.preventDefault();
});






