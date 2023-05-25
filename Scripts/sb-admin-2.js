/*!
 * Start Bootstrap - SB Admin 2 v3.3.7+1 (http://startbootstrap.com/template-overviews/sb-admin-2)
 * Copyright 2013-2016 Start Bootstrap
 * Licensed under MIT (https://github.com/BlackrockDigital/startbootstrap/blob/gh-pages/LICENSE)
 */
$(function () {
    $('#side-menu').metisMenu();
});

//Loads the correct sidebar on window load,
//collapses the sidebar on window resize.
// Sets the min-height of #page-wrapper to window size
$(function() {
    $(window).bind("load resize", function() {
        var topOffset = 50;
        var width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;
        if (width < 768) {
            $('div.navbar-collapse').addClass('collapse');
            topOffset = 100; // 2-row-menu
        } else {
            $('div.navbar-collapse').removeClass('collapse');
        }

        var height = ((this.window.innerHeight > 0) ? this.window.innerHeight : this.screen.height) - 1;
        height = height - topOffset - 66;
        if (width > 767) {
            width = width - 250;
            height = height + 18;
        }
        if (height < 1) height = 1;
        if (width < 1) width = 1;
        if (height > topOffset) {
            $(".fixed-right").css("height", (height) + "px");
        }
        $(".fixed-right").css("width", (width) + "px");
    });

    var url = window.location;
    // var element = $('ul.nav a').filter(function() {
    //     return this.href == url;
    // }).addClass('active').parent().parent().addClass('in').parent();
    var element = $('ul.nav a').filter(function() {
        return this.href == url;
    }).addClass('active').parent();

    while (true) {
        if (element.is('li')) {
            element = element.parent().addClass('in').parent();
        } else {
            break;
        }
    }

    //Menu Dropdown
    $('.nav .dropdown-item').on('click', function (e) {
        var $el = $(this).children('.dropdown-toggle');
        var $parent = $el.offsetParent(".dropdown-menu");
        $(this).parent("li").toggleClass('open');

        if (!$parent.parent().hasClass('navbar-nav')) {
            if ($parent.hasClass('show')) {
                $parent.removeClass('show');
                $el.next().removeClass('show');
                $el.next().css({ "top": -999, "left": -999 });
            } else {
                $parent.parent().find('.show').removeClass('show');
                $parent.addClass('show');
                $el.next().addClass('show');
                $el.next().css({ "top": $el[0].offsetTop, "left": $parent.outerWidth() - 4 });
            }
            e.preventDefault();
            e.stopPropagation();
        }
    });

    $('.nav .dropdown').on('hidden.bs.dropdown', function () {
        $(this).find('li.dropdown').removeClass('show open');
        $(this).find('ul.dropdown-menu').removeClass('show open');
    });
});

$(document).keydown(function (event) {
    if (event.keyCode == 123) { // Prevent F12
        return false;
    } else if (event.ctrlKey && event.shiftKey && event.keyCode == 73) { // Prevent Ctrl+Shift+I        
        return false;
    }
});

function AlertMessage(strMessage)
{
    $('#alert-message').html("");
    $('#alert-message').html(strMessage);
    $('#myModalMessage').modal("show");
}
function ItpOverlay(id) {

    this.id = id;

    /**
	 * Show the overlay
	 */
    this.show = function (id) {

        if (id) {
            this.id = id;
        }

        // Gets the object of the body tag
        var bgObj = document.getElementById(this.id);

        // Adds a overlay
        var oDiv = document.createElement('div');
        oDiv.setAttribute('id', 'itp_overlay');
        oDiv.setAttribute("class", "black_overlay");
        oDiv.style.display = 'block';
        bgObj.appendChild(oDiv);

        // Adds loading
        var lDiv = document.createElement('div');
        lDiv.setAttribute('id', 'loading');
        lDiv.setAttribute("class", "loading");
        lDiv.style.display = 'block';
        bgObj.appendChild(lDiv);

    }

    /**
	 * Hide the overlay
	 */
    this.hide = function (id) {

        if (id) {
            this.id = id;
        }

        var bgObj = document.getElementById(this.id);

        // Removes loading 
        var element = document.getElementById('loading');
        bgObj.removeChild(element);

        // Removes a overlay box
        var element = document.getElementById('itp_overlay');
        bgObj.removeChild(element);
    }

}


function CKUpdateForm() {
    for (instance in CKEDITOR.instances)
        CKEDITOR.instances[instance].updateElement();
}

function RedirecToUrl(strURL)
{
    location.href = strURL;
}