﻿

//this function will show the top picked songs
function showTopPicks() {

    document.getElementById("home_first").style.display = "none";
    document.getElementById("featured").style.display = "none";
    document.getElementById("genres").style.display = "none";
    document.getElementById("account_screen").style.display = "none";
    document.getElementById("list_search_songs").style.display = "none";
    document.getElementById("topPicks").style.display = "initial";
}

//this function will show the featured songs
function showFeatured() {

    document.getElementById("home_first").style.display = "none";
    document.getElementById("topPicks").style.display = "none";
    document.getElementById("genres").style.display = "none";
    document.getElementById("account_screen").style.display = "none";
    document.getElementById("list_search_songs").style.display = "none";
    document.getElementById("featured").style.display = "initial";
}

//this function shows the genres of music we offer
function showGenres() {

    document.getElementById("home_first").style.display = "none";
    document.getElementById("topPicks").style.display = "none";
    document.getElementById("featured").style.display = "none";
    document.getElementById("account_screen").style.display = "none";
    document.getElementById("list_search_songs").style.display = "none";
    document.getElementById("genres").style.display = "initial";
}

//this function shows the user homepage 
function showHome() {

    document.getElementById("topPicks").style.display = "none";
    document.getElementById("featured").style.display = "none";
    document.getElementById("genres").style.display = "none";
    document.getElementById("account_screen").style.display = "none";
    document.getElementById("list_search_songs").style.display = "none";
    document.getElementById("home_first").style.display = "initial";
    document.getElementById("PlaylistsFromDB").style.display = "none";

}

//this function shows the user's accoutn page
function showAccount() {

    document.getElementById("topPicks").style.display = "none";
    document.getElementById("featured").style.display = "none";
    document.getElementById("genres").style.display = "none";
    document.getElementById("home_first").style.display = "none";
    document.getElementById("list_search_songs").style.display = "none";
    document.getElementById("account_screen").style.display = "initial";
}

//this function displays songs when a user searches something
function showSearchedSong() {

    document.getElementById("list_search_songs").style.display = "initial";
    document.getElementById("topPicks").style.display = "none";
    document.getElementById("featured").style.display = "none";
    document.getElementById("genres").style.display = "none";
    document.getElementById("home_first").style.display = "none";
    document.getElementById("account_screen").style.display = "none";
    document.getElementById("PlaylistsFromDB").style.display = "none";
}

var aAudio;
var bAudio;

//in this function a song name is passed, which is played in the audio player and also showed in the now playing area
function myAudioFunction1(song, id, audio, notUserPlaylistSong) {

    var check = document.getElementById(id + song).innerHTML;
    var audio = document.getElementById(id + audio).innerHTML;

    // if the song is not one of the default songs within the project, then the song name is sent here
    if (song == check) {
        bAudio = audio;
        player = document.getElementById('audio_player');
        player.src = bAudio;
        player.play();
        document.getElementById('song-name').innerHTML = check;
    }

    if(notUserPlaylistSong === "true")
    {
        $.ajax({
            type: "POST",
            url: '/Home/UpdateSongPlayed',
            dataType: "html",
            data: {
                SongID: id
            },
            success: function (data) {
            },
            error: function (jqXHR, textStatus) {
                //alert(textStatus);
            }

        });
    }
}

function playSong(song, audio, id) {
    aAudio = audio;
    player = document.getElementById('audio_player');
    player.src = aAudio;
    player.play();
    document.getElementById('song-name').innerHTML = song;

    $.ajax({
        type: "POST",
        url: '/Home/UpdateSongPlayed',
        dataType: "html",
        data: {
            songID: id
        },
        success: function (data) {
        },
        error: function (jqXHR, textStatus) {
            //alert(textStatus);
        }

    });
}

//this function is for the user's image URL
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#placeholderImage')
                .attr('src', e.target.result);
        };

        reader.readAsDataURL(input.files[0]);
    }
}

function reloadPage() {
    window.location.reload();
}

//this function shows the user's password when they click on the show password button
function showPassword() {
    var x = document.getElementById("UPassword");
    if (x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
}

var searchbutton = document.getElementById("lookupsong");
var searchsomething = document.getElementById("searchsong");

//this message is displayed in case the user clicks search button without typing in anything to search
searchbutton.addEventListener("click", function () {
    if (searchsomething.value == "") {
        alert("Please enter a song to search.");
    }
    else {
        showSearchedSong();
    }
});

function deleteSongs(songToDeleteID, songNameDelete) {

    $.ajax({
        type: "POST",
        url: '/Home/deleteSong',
        dataType: "html",
        data: {
            deleteSong: songToDeleteID,
            songName: songNameDelete
        },
        success: function (data) {
            alert("Deleting song: " + songNameDelete);
            window.location.reload();
        },
        error: function (jqXHR, textStatus) {
            //alert(textStatus);
        }

    }); 
}

function deletePlaylists(PlaylistToDeleteID, PlaylistNameDelete) {

    $.ajax({
        type: "POST",
        url: '/Home/deletePlaylist',
        dataType: "html",
        data: {
            deletePlaylist: PlaylistToDeleteID,
            PlaylistName: PlaylistNameDelete
        },
        success: function (data) {
            alert("Deleting playlist: " + PlaylistNameDelete);
            window.location.reload();
        },
        error: function (jqXHR, textStatus) {
            //alert(textStatus);
        }

    });
}


function AddSongToPlaylist(songToAddAudio, songNameAdd) {
    document.getElementById("home_first").style.display = "none";
    document.getElementById("loading").style.display = "none";
    document.getElementById("loading").style.display = "hidden";
    document.getElementById("PlaylistsFromDB").style.display = "none";
    document.getElementById("featured").style.display = "none";
    document.getElementById("topPicks").style.display = "none";
    document.getElementById("list_add_songs_to_playlist").style.display = "initial";
    sessionStorage.setItem("songAudioAdd", songToAddAudio);
    sessionStorage.setItem("songNameAdd", songNameAdd);
}


function selectPlaylist(ToPlaylistAddID, ToPlaylistName)
{
    $.ajax({
        type: "POST",
        url: '/Home/addSongToPlaylist',
        dataType: "html",
        data: {
            PlaylistID: ToPlaylistAddID,
            songName: sessionStorage.getItem("songNameAdd"),
            addSongAudio: sessionStorage.getItem("songAudioAdd"),
            playListName: ToPlaylistName
        },
        success: function (data) {
            let songNameAdded = sessionStorage.getItem("songNameAdd");
            document.getElementById("list_add_songs_to_playlist").style.display = "none";
            document.getElementById("home_first").style.display = "initial";
            alert(data);
            window.location.reload();
        },
        error: function (jqXHR, textStatus) {
            //alert(textStatus);
        }

    });
}

//This function will show the playlists and the list of songs in them
function showSongs(playlistNumber, currentPlaylistID, Playlist_Name) {

    var ArrayOfLists = playlistNumber.split('.');

    for (let i = 0; i < ArrayOfLists.length; i++) {
        if (ArrayOfLists[i] == currentPlaylistID) {
            var Id = "songsinplaylist" + currentPlaylistID;
            $.ajax({
                type: "POST",
                url: '/Home/GetID',
                dataType: "html",
                data: {
                    dataID: currentPlaylistID,
                },
                success: function (data) {
                    var arr_from_json = JSON.parse(data);
                    for (var i in arr_from_json) {
                        //alert(arr_from_json[i].Song_Name);
                    }
                },
                error: function (jqXHR, textStatus) {
                    //alert(textStatus);
                }

            });
            break;
        }
    }

    document.getElementById("home_first").style.display = "none";
    document.getElementById("loading").style.display = "none";
    document.getElementById("loading").style.display = "hidden";
    document.getElementById("PlaylistsFromDB").style.display = "initial";
    document.getElementById("PlaylistName").innerHTML = Playlist_Name;
}

function PageLoading() {
    setTimeout(function () {
        location.reload();
    }, 2000);
    document.getElementById("home_first").style.display = "none";
    document.getElementById("PlaylistsFromDB").style.display = "none";
    document.getElementById("loading").style.display = "initial";
    document.getElementById("loaderWheel").style.visibility = "visible";
}



//this is the back button which will take you back to the user homepage
function gobacktoplaylists() {
    document.getElementById("home_first").style.display = "initial";
    document.getElementById("list_search_songs").style.display = "none";
    document.getElementById("PlaylistsFromDB").style.display = "none";
    document.getElementById("list_add_songs_to_playlist").style.display = "none";
}

//this function will add the list item as playlist and will use the user input as the playlist's name 
function AddLi(str) {
    var li = document.createElement('li');
    li.appendChild(document.createTextNode(str));
    li.innerHTML += ' <button id="deletePlaylist" onclick="this.parentNode.remove()">Delete Playlist</button>';
    document.getElementById("out").appendChild(li);
}

//this function will add a list item as a song that a user will try to upload ***This is hard coded,we could not implement it in time ***
function anotherSong() {
    var li = document.createElement('li');
    li.appendChild(document.createTextNode('Another Song'));
    document.getElementById("moreSongs").appendChild(li);
}

function downloadSong(song) {
    window.open(song, '_blank');
}