// JavaScript Document
(function ($) {
	"use strict";

	// scroll off when page load
	$("#main").removeClass("body-scroll");
	$("#main").addClass("body-noscroll");

	/* ==========================================================================
	   Top Menu click and scroll	
	   ========================================================================== */
	$(document).on('click', 'a.page-scroll', function (event) {
		var $anchor = $(this);
		$('html, body').stop().animate({
			scrollTop: $($anchor.attr('href')).offset().top - 50
		}, 1500, 'easeInOutExpo');
		event.preventDefault();
	});

	/* ==========================================================================
	   Highlight the top nav as scrolling occurs	
	   ========================================================================== */
	$('body').scrollspy({
		target: '.navbar-fixed-top',
		offset: 51
	})

	/* ==========================================================================
	   ScrollUp Active Code
	   ========================================================================== */
	$.scrollUp({
		scrollName: 'scrollUp',
		scrollDistance: 450,
		scrollFrom: 'top',
		scrollSpeed: 500,
		easingType: 'linear',
		animation: 'fade',
		animationSpeed: 200,
		scrollTrigger: false,
		scrollTarget: false,
		scrollText: '<i class="fa fa-chevron-up"></i>',
		scrollTitle: false,
		scrollImg: false,
		activeOverlay: false,
		zIndex: 2147483647
	});

	/* ==========================================================================
	   Banner Rotater
	   ========================================================================== */
	var bannerRotater = $("#introSlider3");
	bannerRotater.owlCarousel({
		animateIn: 'fadeIn',
		animateOut: 'fadeOut',
		lazyLoad: true,
		loop: true,
		autoplay: true,
		autoplayTimeout: 6000, //Set AutoPlay to 6 seconds			 
		items: 3,
		responsive: {
			0: {
				items: 1,
			},
			1000: {
				items: 1,
			}
		}
	});

	var leftnav = $(".introcarousel-lft-nav");
	leftnav.on('click', function (e) {
		bannerRotater.trigger('prev.owl.carousel');
	});

	var rightnav = $(".introcarousel-rgt-nav");
	rightnav.on('click', function (e) {
		bannerRotater.trigger('next.owl.carousel');
	});

	bannerRotater.on('translate.owl.carousel', function (event) {
		$('.owl-item .item .intro-single-content h2').removeClass('animated').hide();
		$('.owl-item .item .intro-single-content p').removeClass('animated').hide();
		$('.owl-item .item .intro-single-content .btn').removeClass('animated').hide();
	});

	bannerRotater.on('translated.owl.carousel', function (event) {
		$('.owl-item.active .item .intro-single-content h2').addClass('animated fadeInUp').show();
		$('.owl-item.active .item .intro-single-content p').addClass('animated fadeInDown').show();
		$('.owl-item.active .item .intro-single-content .btn').addClass('animated fadeInDown').show();
	});

	/* ==========================================================================
	   Our History Slider
	   ========================================================================== */
	var HistorySlider = $("#historySlider");
	HistorySlider.owlCarousel({
		loop: true,
		autoplay: true,
		autoplayTimeout: 3000, //Set AutoPlay to 3 seconds
		items: 3,
		autoplayHoverPause: true,
		responsive: {
			0: {
				items: 1,
			},
			1000: {
				items: 3,
			}
		}
	});

	var history_left = $(".history-lft-nav")
	history_left.on('click', function (e) {
		HistorySlider.trigger('prev.owl.carousel');
	});

	var history_right = $(".history-rgt-nav")
	history_right.on('click', function (e) {
		HistorySlider.trigger('next.owl.carousel');
	});
	HistorySlider.on('mouseleave', function (e) {
		HistorySlider.trigger('next.owl.carousel');
	});

	/* ==========================================================================
	   Top Menu Navigation effect
	   ========================================================================== */
	var nav = $('.navbar-inverse');
	$(window).scroll(function () {
		if ($(this).scrollTop() > 60) {
			nav.addClass('navbar-default');
			nav.removeClass("navbar-inverse");
		} else {
			nav.addClass("navbar-inverse");
			nav.removeClass('navbar-default');
		}
	});

	/* ==========================================================================
	   Portfolio Image Gallery
	   ========================================================================== */
	$('.filtr-container').imagesLoaded(function () {
		// images have loaded
		//Initialize filterizr with default options
		$('.filtr-container').filterizr();

		// Initialize & Configure : ColorBox [Gallery]
		$(".gallery").colorbox({
			rel: 'gallery',
			close: '',
			previous: '',
			next: '',
			current: 'image {current} / {total}',
			fixed: true
		});
	});

	/* ==========================================================================
	   Init pre-loader and set the top naviagtion menu
	   ========================================================================== */
	$(window).on("load", function () {
		/* Pre Loader */
		$('#preloader').fadeOut('medium', function () {
			$(this).remove();
			// scroll on after page loaded all elements
			$("#main").removeClass("body-noscroll");
			$("#main").addClass("body-scroll");
		});

		// Top Scroll Menu
		var nav = $('.navbar-inverse');
		if ($(this).scrollTop() > 60) {
			nav.addClass('navbar-default');
			nav.removeClass("navbar-inverse");
		} else {
			nav.addClass("navbar-inverse");
			nav.removeClass('navbar-default');
		}
	});

	/* ==========================================================================
	   set image height for full-page-slider
	   ========================================================================== */
	function setHeight() {
		var windowHeight = $(window).innerHeight();
		$('.carousel-fullpage').css('min-height', windowHeight);
	};

	$(window).on("resize", function () {
		setHeight();
	});

	/* ==========================================================================
	   Page Counter
	   ========================================================================== */
	var $findme = $('#pageCounter');
	var exec = false;

	function animatedNumberCounter() {

		$findme.each(function () {

			var $section = $(this),
				findmeOffset = $section.offset(),
				findmeTop = findmeOffset.top,
				findmeBottom = $section.height() + findmeTop,
				scrollTop = $(document).scrollTop(),
				visibleBottom = window.innerHeight,
				prevVisible = $section.prop('_visible'),
				visible = false;

			if ((findmeTop > scrollTop + visibleBottom) ||
				findmeBottom < scrollTop) {
				visible = false;
			} else visible = true;

			if (!prevVisible && visible) {
				if (!exec) {
					$('.count').each(function () {
						$(this).prop('Counter', 0).animate({
							Counter: $(this).text()
						}, {
							duration: 4000,

							step: function (now) {
								$(this).text(Math.ceil(now));
								exec = true;
							}
						});
					});
				}
			}
			$section.prop('_visible', visible);
		});

	}

	function animatedNumberCounterSetup() {
		var $top = $('#top');
		var $bottom = $('#bottom');

		$top.height(500);
		$bottom.height(500);

		$(window).scroll(function () {
			animatedNumberCounter();
		});
	}

	/* ==========================================================================
	   Document ready actions
	   ========================================================================== */
	$(document).on('ready', function () {

		// $("#beginning").mCustomScrollbar();

		// Init Banner
		bannerRotater.owlCarousel();

		// set Banner slider size fit into fullpage after page load
		setHeight();

		// Init WOW Effects
		new WOW().init();

		$('.filtr-container').filterizr('filter', 'all');

		// Animated Number Counter
		animatedNumberCounterSetup();
		
	});

})(jQuery);

/* ==========================================================================
   Assign you-tube URL when user click the Watch video play button
   ========================================================================== */
function loadYoutube() {
	$('#video_src').attr("src", "https://www.youtube.com/embed/HndV87XpkWg");
}
