	$(document).ready(function() {


		$.expr[":"].containsNoCase = function(el, i, m) {
			var search = m[3];
			if (!search) return false;
			return eval("/" + search + "/i").test($(el).text());
		};

		// Clear Agency Search Box
		$('.clear-search').hide();

		var $searchBox = $('.search-agencies');

		// Clear Agency Search Box
		$('.clear-search').on('click', function(evt) {
			$searchBox.val('');
			$(this).hide();
			
			$('.agency-section, .agency-section h2, .agency-section li').show();
			$('.agency-section').removeClass('alt');
			$('.agency-section h2').removeClass('noHover');

			$('.agency-section .container').delay(400).slideUp();

			evt.preventDefault();
		});
		

		$searchBox.keyup(function() {
			
			// What did they type
			var $length = $(this).val().length;
			

				// Did someone type something?
				if ($length >= 3) {
					// show the clear link
					$('.clear-search').fadeIn('slow');
					
					// Hide all of the page results			
					$('.agency-section, .agency-section li').hide();
					$('.agency-section h2').addClass('noHover');

					// the results
					var $theResults = $('.agency-section li:containsNoCase(\'' + $(this).val() + '\')');

					// Show the results
					$theResults.show();
					$theResults.closest('.agency-section').show().removeClass('alt');
					$theResults.closest('h2').show();
					$theResults.closest('.agency-section .container').show();
				
				} else {
					$('.clear-search').hide();
					
					// If the value is less than one, show the content
					$('.agency-section, .agency-section li, .agency-section h2').show();
					$('.agency-section').removeClass('alt');
					$('.agency-section h2').removeClass('noHover');
					$('.agency-section .container').hide();
				}
		});



		// Accordion agencylist.html
		var $agencyTitle = $('.agency-section h2');
		$agencyTitle.siblings('.container').delay(400).slideUp(600);
		

		// Loop through each agency section title
		$agencyTitle.each(function() {

			$(this).on('click', function() {
				if (!$(this).hasClass('noHover')) {
					
					// $(this).addClass('noHover');
					$(this).parent().toggleClass('alt');
					
					var $otherSections = $(this).parent().siblings().not($(this).parent());

					$otherSections.removeClass('alt');
					$otherSections.children('.container').slideUp();

					$(this).siblings('.container').slideToggle();
				}
			});
		});


		// citycounty.html option value links 
		$('#countymenu, #citymenu').bind('change', function (evt) {
        	var url = $(this).val(); // get selected value
        	if (url != '') { // require a URL
            	window.location = url; // redirect
        	}
        	evt.preventDefault();
    	});

	});