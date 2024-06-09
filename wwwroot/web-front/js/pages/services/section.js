// remap jQuery to $
(function($){


/* trigger when page is ready */
$(document).ready(function (){

	$('#typeFilters #TypeFilterAll').click(function() {
		mindexUpdate('gcall');
	});

	$('#typeFilters #TypeFilterState').click(function() {
		mindexUpdate('gcstate');
	});

	$('#typeFilters #TypeFilterFed').click(function() {
		mindexUpdate('gcfed');
	});

	$('#typeFilters #TypeFilterLocal').click(function() {
		mindexUpdate('gclocal');
	});

});


/* optional triggers

$(window).load(function() {
	
});

$(window).resize(function() {
	
});

*/


})(window.jQuery);