
//
//  Custom javascript goes in here
//

$(function () {
    "use strict";    
    
    /* Sidebar tree view */
    $(".sidebar .treeview").tree();    

});

(function ($) {
    "use strict";

    $.fn.tree = function () {

        return this.each(function () {
            var btn = $(this).children("a").first();
            var menu = $(this).children(".treeview-menu").first();
            var isActive = $(this).hasClass('active');

            //initialize already active menus
            if (isActive) {
                menu.show();
                btn.children(".ion-ios-arrow-down").first().removeClass("ion-ios-arrow-down").addClass("ion-ios-arrow-up");
            }
            //Slide open or close the menu on link click
            btn.click(function (e) {
                e.preventDefault();
                if (isActive) {
                    //Slide up to close menu
                    menu.slideUp();
                    isActive = false;
                    btn.children(".ion-ios-arrow-up").first().removeClass("ion-ios-arrow-up").addClass("ion-ios-arrow-down");
                    btn.parent("li").removeClass("active");
                } else {
                    //Slide down to open menu
                    menu.slideDown();
                    isActive = true;
                    btn.children(".ion-ios-arrow-down").first().removeClass("ion-ios-arrow-down").addClass("ion-ios-arrow-up");
                    btn.parent("li").addClass("active");
                }
            });

            /* Add margins to submenu elements to give it a tree look */
            // menu.find("li > a").each(function () {
                // var pad = parseInt($(this).css("margin-left")) + 10;

                // $(this).css({ "margin-left": pad + "px" });
            // });

        });

    };


} (jQuery));


/* To keep the treeview menu open after postback */
$(document).ready(function () {
    var url = stripQueryStringAndHashFromPath(this.location.pathname);
    var mnu = $('a[href*="' + url + '"]').parent().parent();	
    if (mnu.hasClass('dropdown-menu') == false) {
		$('a[href*="' + url + '"]').addClass('active');
        mnu.parent().parent().parent().addClass('active');
        mnu.addClass('active');
        mnu.parent().addClass('active');
        mnu.attr('style', 'display:block');
    }
});

function stripQueryStringAndHashFromPath(url) {
    return url.split("?")[0].split("#")[0];
}

/* End of --- To keep the treeview menu open after postback */
