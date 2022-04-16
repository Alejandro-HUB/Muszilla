//this function will show the registration form if it is not already displaying
function showRegistration() {
    var x = document.getElementById("cb");           //back of card
    var y = document.getElementById("cf");          //front of card
    if (x.style.display === "none") {
        x.style.display = "block";
        y.style.display = "none";
        document.getElementById('loginForm').reset();
    }
    else {
        x.style.display = "none";
    }
}

//this function will show the login form if it is not already displaying
function showLogin() {
    var x = document.getElementById("cb");           //back of card
    var y = document.getElementById("cf");          //front of card
    if (y.style.display === "none") {
        y.style.display = "block";
        x.style.display = "none";
        document.getElementById('registration').reset();
    }
    else {
        y.style.display = "none";
    }
}

function onSignIn(googleUser) {
    var profile = googleUser.getBasicProfile();
    console.log('ID: ' + profile.getId()); // Do not send to your backend! Use an ID token instead.
    console.log('FirstName: ' + profile.getGivenName());
    console.log('LastName: ' + profile.getFamilyName());
    console.log('Image URL: ' + profile.getImageUrl());
    console.log('Email: ' + profile.getEmail()); // This is null if the 'email' scope is not present.

    $.ajax({
        type: "POST",
        url: '/Home/GoogleLogin',
        dataType: "html",
        data: {
            FirstName: profile.getGivenName().toString(),
            LastName: profile.getFamilyName().toString(),
            ImageURL: profile.getImageUrl().toString(),
            email: profile.getEmail().toString(),
        },
        success: function (data) {
            window.location = '/Home/Homepage';
        },
        error: function (jqXHR, textStatus) {
            //alert(textStatus);
        }

    });
}