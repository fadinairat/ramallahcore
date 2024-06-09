// remap jQuery to $
(function($){

	$(document).ready(function () {

		// // Hurricane Relief Button
		// var mariaRelief = document.querySelector('.maria-relief');
		// var mariaReliefClose = document.querySelector('.maria-relief-close');

		// mariaReliefClose.onclick = function(e) {
		// 	e.preventDefault();
		// 	mariaRelief.classList.add('hidden');
		// };

		// Some Global Variables
		var $backToTop = $('#backToTop');
		var $dropDown = $('.drop-down');
		var width = $(window).width();
		var $noScroll = false;


		// Hide elements on page load
		$('section header, section .content-container').addClass('hidden');

		
		// Toggle the Mobile Menu
		$('.toggle-menu, #overlay').on('click', function() {

			if ( $('body').hasClass('showMenu') ) {
				$('body, html, #main-menu').removeClass('showMenu');
				$('.toggle-menu').removeClass('active');
			} else {
				$('body, html, #main-menu').addClass('showMenu');
				$('.toggle-menu').addClass('active');
			}

			// Close Sub-menus
			if ( $('#main-menu > ul > li').hasClass('open') ) {
				$('li').removeClass('open');
				$('.drop-down').slideUp();
				$('nav#main-menu').removeClass('open-ie');
			}
		});

		
		// Toggle the .drop-down menu "Desktop" & "Mobile"
		$('#main-menu > ul > li:not(#nav-home) > a').on('click', function(e) {

			$('#socialmedialist').removeClass('open');

			// remove Ajax search box
			if (srch.kgb.active == false) srch.mod.reset();

			if ( $(this).parent().hasClass('open') ) {
				$(this).next('.drop-down').slideUp(240);
				$(this).parent().removeClass('open');
				$('nav#main-menu').removeClass('open-ie');
			} else {
				// slideUp ALL the other Menus
				$('#main-menu > ul li .drop-down').slideUp(240);

				if ( width >= 769 ) {
					$('nav#main-menu').addClass('open-ie');
				}
				
				// DELAY the .drop-down slideDown IF ("desktop" AND there's already an open menu)
				if ( width > 768 && $('#main-menu > ul > li:not(#nav-home)').hasClass('open') ) {
					$(this).next('.drop-down').delay(420).slideDown();
				// otherwise just slide down the drop-down
				} else {
					$(this).next('.drop-down').slideDown();
				}

				// remove .open class from all Menus
				$('#main-menu > ul li').removeClass('open');
				// ADD .open class to this Menu
				$(this).parent().addClass('open');
			}
			if($(this).parent().hasClass("hasSub"))
				e.preventDefault();
		});


		/* //////////////////////
		RESIZE WINDOW / page LOAD
		////////////////////// */
		$(window).on('resize load', function() {

			width = $(window).width(); // reset the 'width' variable
			height = $(window).height(); // reset the 'height' variable


			// prevent ios Scrolling the webview on search focus
			if ( width <= 768 ) {
				$('#searchField').on('focus', function(e) {
					$noScroll = true;
				});
			}

			// if > Tablet width
			if ( width > 768 ) {
				// Shut down the mobile nav stuff
				$('body, html').removeClass('showMenu');
				$('#main-menu > ul li .drop-down').slideUp();
				$('#main-menu > ul li').removeClass('open');
				$('nav#main-menu').removeClass('open-ie');
			}

		

			/* //////////////////////
			SCROLLING
			////////////////////// */

			$('#goToCity').on('click', function() {
				$(this).fadeOut('slow');
				$('html, body').animate({scrollTop: $('#locals').offset().top}, 1200);
			});
			

			// Home Page Section Navigation
			$('.section-nav a').on('click', function(e) {
				e.preventDefault();

				var theSectionID = $(this).attr('href');
				
				$('html, body').animate({
					scrollTop: $(theSectionID).offset().top
				}, 600);

				$('.section-nav a').removeClass('active');
				$(this).addClass('active');
			});
			
			var scrollTimeout;  // global for any pending scrollTimeout
			var lastScrollTop = 0; // set to detect scroll direction
			
			$(window).scroll(function () {
				if (scrollTimeout) {
					// clear the timeout, if one is pending
					clearTimeout(scrollTimeout);
					scrollTimeout = null;
				}
				scrollTimeout = setTimeout(scrollHandler, 120);
			});
			
			
			scrollHandler = function () {

				var $viewport = $('body, html');
				
				//the current scroll position
				var $y = $(window).scrollTop();


				// Close Sub-menus
				if ( $('#main-menu > ul > li:not(#nav-home)').hasClass('open') && $noScroll == false ) {
					$('li').removeClass('open');
					$('.drop-down').slideUp();
					$('nav#main-menu').removeClass('open-ie');
				}

				// Close the "Share this page links"
				$('#socialmedialist').removeClass('open');

				// Hide the settings div
				if ( $(window).scrollTop() > 55 ) {
					$('#settings, #socialmedialist').removeClass('open');
					$('body > header nav').removeClass('settingsOpen');
				}

				// Reset Section nav highlight
				if ( $y < 400 ) {
					$('.section-nav a').removeClass('active');
				}

				// Shrink-ify Header on scroll
				if ( $y > 32 ) {
					//$('#main-header').addClass('toTop');
				} else {
					//$('#main-header').removeClass('toTop');
				}

				// Show the backToTop link
				if ( $y > 400) {
					$backToTop.addClass('show');
				} else {
					$backToTop.removeClass('show');
				}

				


				/* //////////////////////
				Scroll to HOME PAGE <SECTIONS>
				////////////////////// */
				
					// highlight search section in .section-nav
					if ( $y < 100 ) {
						$('.section-nav a.search').addClass('active');
					}
					
					// loop through the home page sections
					$('section').each(function() {

						// get y-position for each home page section 
						var thisTop = $(this).offset().top;

						// Highlight the current section in the .section-nav
						var theSection = $(this).attr('id');
							
						$viewHeight = $(this).height();
						
						// Add .scrolled class to section to fire CSS animations/transitions
						if ( $y + 128 > thisTop ) {
							$(this).addClass('scrolled');
							$(this).children('header, .content-container').removeClass('hidden');
						} //else if ( $y < thisTop ) {
						// 	$(this).removeClass('scrolled');
						// 	$(this).children('header, .content-container').addClass('hidden');
						// }

						// animate scroll to top of section
						
						// Based on viewport height
						var yDetect;

						// if the viewport is tall
						if (height >= 700) {
							yDetect = 640;
						// if the viewport is short
						} else {
							yDetect = 380;
						}

						// if scrolling DOWN the page
						if ( $y > lastScrollTop && $y > (thisTop - yDetect) && $y <= (thisTop) ) {
							//$viewport.animate({ scrollTop: thisTop }, 500);
							
							// highlight the current section in .section-nav
							$('.section-nav a').removeClass('active');
							$('.section-nav a.' + theSection).addClass('active');

						// if scrolling UP the page
						} else if ( $y < lastScrollTop && $y < (thisTop + yDetect) && $y >= (thisTop) ) {
							//$viewport.animate({ scrollTop: thisTop }, 500);

							// highlight the current section in .section-nav
							$('.section-nav a').removeClass('active');
							$('.section-nav a.' + theSection).addClass('active');
						}

					});// /for each section
			
				

				// reset "last scroll" position for scroll DIRECTION detection
				lastScrollTop = $y;
			
			}; // /scrollHandler

		}); // /window.load.resize
		

		$('#backToTop').click(function(e) {
			$('body,html').animate({scrollTop: 0}, 1200);
			$('.section-nav a').removeClass('active');
			// e.preventDefault();
		});


		// Share show #socialmedialist
		$('#socialShare').click(function(e) {
			$('#socialmedialist').toggleClass('open');
			e.preventDefault();
		});


		/* SETTINGS
		///////////////////////////////////// */
		var settings = localStorage.getItem('settings');

		if (settings==null) {
			localStorage.setItem('settings', 'default');
			settings = 'default';
		}

		if (settings == 'highContrast') {
			$('html').addClass('highContrast');
		} else if (settings == 'textOnly') {
			$('html').addClass('textOnly');
		}


		// Show Settings (Location, Style Switcher & Font Sizer)
		$('a#toggleSettings, a#toggleLocation').click(function(e) {
			
			var $nav = $('body > header nav');
			var $settings = $('#settings');

			// Click location
			if ( $(this).attr('title') == 'Location' && !$settings.hasClass('open') && !$settings.hasClass('settings') ) {
				$settings.addClass('location open');
				$nav.addClass('settingsOpen');
			
			} else if ( $(this).attr('title') == 'Location' && $settings.hasClass('open') && $settings.hasClass('settings') ) {
				$settings.removeClass('settings').addClass('location');
			
			} else if ( $(this).attr('title') == 'Location' && $settings.hasClass('open') && $settings.hasClass('location') ) {
				console.log("settings is open with the class of location");
				$settings.removeClass('open location');
				$nav.removeClass('settingsOpen');
			
			// Click settings
			} else if ( $(this).attr('title') == 'Settings' && !$settings.hasClass('open') && !$settings.hasClass('location') ) {
				$settings.addClass('open settings');
				$nav.addClass('settingsOpen');
			
			} else if ( $(this).attr('title') == 'Settings' && $settings.hasClass('open') && $settings.hasClass('location') ) {
				$settings.removeClass('location').addClass('settings');

			} else if ( $(this).attr('title') == 'Settings' && $settings.hasClass('open') && $settings.hasClass('settings') ) {
				$settings.removeClass('open settings');
				$nav.removeClass('settingsOpen');
			}
			
			e.preventDefault();
		});


		// Settings Style Switcher
		$('.settings a').on('click', function(e) {
			var setting = $(this).attr('class');
			
			$('html').removeClass('textOnly highContrast');
			$('html').addClass(setting);
			localStorage.setItem('settings', setting);

			$('#settings a').removeClass('active');
			$(this).addClass('active');
			
			e.preventDefault();
		});



		// Local Storage for Font size settings
		var settings = localStorage.getItem('settings');

		// If Default size
		if (settings=='undefined' || settings=='default') {
			$('#fontSizer .fontsize').removeClass('active');
			$('#fontSizer .fontsize.default').addClass('active');
			localStorage.setItem('settings', 'default');
			settings = 'default';
		}

		// If Small size
		if (settings == 'smallFont') {
			$('#fontSizer .fontsize').removeClass('active');
			$('#fontSizer .fontsize.small').addClass('active');
			$('html').addClass('smallFont');
		
		// If Large size
		} else if (settings == 'largeFont') {
			$('#fontSizer .fontsize').removeClass('active');
			$('#fontSizer .fontsize.large').addClass('active');
			$('html').addClass('largeFont');
		}

		// Change/Set Font Size
		$('#fontSizer .fontsize').on('click', function(e) {
			var size = $(this).data('size');
			
			$('#fontSizer .fontsize').removeClass('active');
			$(this).addClass('active');
			
			$('html').removeClass('largeFont smallFont');
			$('html').addClass(size);
			
			localStorage.setItem('settings', size);
			e.preventDefault();
		});


		// Click Outside Menu to close .drop-down
		$(document).on('click', function(event) {
			if (!$(event.target).closest('#main-header').length && $dropDown.parent('li').hasClass('open')) {
				$dropDown.parent('li').removeClass('open');
				$dropDown.slideUp();
				$('nav#main-menu').removeClass('open-ie');
			}
		});


		// Open Section Transparency Donut Chart
		$('.transparency-donut path').on('click', function(e) {
			var className = $(this).attr('class');
			var theColor = $(this).attr('fill');
			var listItem = $('li.' + className);

			$('.transItem').removeClass('active').css('background', '');
			listItem.addClass('active').css('background', theColor);
		});

	});// /(document).ready


})(window.jQuery);


//GET URL POST WORD (ANY) - Just pass getURLpost(anyterm) to retrieve from URL
function getURLpost(term) {
	var sPageURL = window.location.search.substring(1);
	var sURLVariables = sPageURL.split('&');

	for (var i = 0; i < sURLVariables.length; i++) {
		var kword = sURLVariables[i].split('=');
		if (kword[0] == term) return kword[1];
	}
}


/*
#### SHARING/SOCIAL MEDIA ####
*/

$(function(){
	//GET AND MAKE PAGE TITLE URL FRIENDLY
	var getPageTitle = $('html title').text();
	var sharingTitle = encodeURIComponent(getPageTitle);
	//console.log(sharingTitle);
	$('#socialmedialist a').each(function() {
		var sharelink = $(this).attr('href');
		var sharelinkwithtitle = sharelink.replace('PAGE_TITLE', sharingTitle, 'g');
		$(this).attr('href', sharelinkwithtitle);
	});
});