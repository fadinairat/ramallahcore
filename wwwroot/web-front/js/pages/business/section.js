// remap jQuery to $
(function($){


/* trigger when page is ready */
$(document).ready(function() {


	$('.industry #mindexfilterCategory').on('change', function() {
		$('.industry-results').slideDown();
	});

});


/* optional triggers

$(window).load(function() {
	
});

$(window).resize(function() {
	
});

*/


})(window.jQuery);