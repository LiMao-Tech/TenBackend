$(document).ready(function () {
    $.getJSON("api/TenUsers", function (data) {
        var count = 0;
        $.each(data, function (i,user) {
            count += isNewUser(user);
            $('#newUser').text(count);
        })
       // console.log(count)
    });
});


function isNewUser(user){
    var date = new Date();
    var now = date.getTime()/1000+date.getTimezoneOffset()*60;
    var joinDate = user["JoinedDate"];
    if((now - joinDate) < 24*60*60){
        return 1;
    }
    return 0;
}

