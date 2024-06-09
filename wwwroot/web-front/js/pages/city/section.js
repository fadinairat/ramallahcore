// remap jQuery to $
(function($){


/* ##################################################################
	ALTMDI (Branch of masterindex.js for CITIES OUTPUT)
################################################################## */

var altmdi = {};
	altmdi.url = '/masterindex/search.json';

altmdi.getParam = function(term) {
	var sPageURL = window.location.search.substring(1);
	var sURLVariables = sPageURL.split('&');

	for (var i = 0; i < sURLVariables.length; i++) {
		var kword = sURLVariables[i].split('=');
		if (kword[0] == term) return kword[1];
	}
}

	altmdi.manualCity = altmdi.getParam('view');
	console.log(altmdi.manualCity);

$('[data-locate=manual-city]').each(function(i) {
	altmdi.manualCity = altmdi.manualCity.replace(/\+/g,' ');
	$(this).text(altmdi.manualCity);
});

/* ##################################################################
	GET CITY LAT/LNG
################################################################## */


altmdi.getGeo = function() {

	$.getJSON(altmdi.url, { keyword: altmdi.manualCity, typeIds: 95, limit: 1 }, function(data) {
		
		altmdi.lat = data.results[0].location.latitude;
		altmdi.lng = data.results[0].location.longitude;

		altmdi.initialize();
	});

}



/* ##################################################################
	INITIALIZE MDI CALL
################################################################## */

altmdi.initialize = function() {

	$('.altmIndex').each(function(index) {

		var args = {};
			args.mdiContainer 	= $(this);
			args.category 		= args.mdiContainer.data('category'); 	// Get the MDI Category
			args.locate 		= args.mdiContainer.data('locate'); 	// if True, use users location.
			
			args.keyword 		= altmdi.manualCity;

			args.type 			= args.mdiContainer.data('type'); 		// Get the MDI Type

			args.lat 			= altmdi.lat; 
			args.lng 			= altmdi.lng; 

			args.radius 		= args.mdiContainer.data('radius'); 	// Get the MDI Location Radius
			args.govclass 		= args.mdiContainer.data('govclass'); 	// Get the MDI Government Class
			args.format 		= args.mdiContainer.data('format'); 	// Get the MDI Type
			args.limit 			= args.mdiContainer.data('limit'); 		// Limit the results
			args.HTML 			= '';

			
			if (args.locate === 'device') args.keyword = args.mdiContainer.data('keyword'); 
			if (args.format === 'background') {
				args.keyword = undefined;
				args.radius = '100';
			}

		// GET DATA
		altmdi.getdata(args);

	});

}; // altmdi.initialize


/* ##################################################################
	CALL MASTER INDEX - GET RESULTS
################################################################## */

altmdi.getdata = function(args) {

	

	// GET FEED
	$.getJSON(altmdi.url, { categoryIds: args.category, keyword: args.keyword, typeIds: args.type, latitude: args.lat, longitude: args.lng, radius: args.radius, governmentClass: args.govclass, limit:args.limit }, function(data) {

	// ################ BUILD FORMATS ################################

		//TRENDING FORMAT
		if (args.format === 'trending') {
			args.HTML = altmdi.buildTrending(data);
		}
		//TRENDING FORMAT
		else if (args.format === 'optionlist') {
			args.HTML = altmdi.buildOptionslist(data);
		}
		//UTAH CITIES FORMAT
		else if (args.format === 'utahcities') {
			args.HTML = altmdi.buildCitiesList(data);
		}
		//DEFAULT FORMAT
		else {
			args.HTML = altmdi.build(data);
		}

	// ################ OPTIONS ################################

		//Get Result Count
		if (args.counter === true) args.counterHTML = altmdi.buildCounter(data);

		//Get More Results BTN
		if (isFinite(args.more) && args.more != '') args.getmoreHTML = altmdi.buildMore(data, args.more);

		// WRITE OUT FEED TO .mIndex ul
		
		// COUNT
		if (args.format === 'count') {
			altmdi.outputResultCount(data, args);
		} 

		// COUNT UP
		else if (args.format === 'countup') {
			altmdi.outputResultCountUp(data, args);
		}

		//BACKGROUND IMAGE FORMAT
		else if (args.format === 'background') {
			altmdi.outputBackground(data, args);
		}

		// NORMAL OUTPUT
		else {
			altmdi.output(data, args);
		}
	});

} // altmdi.getdata


/* ##################################################################
	DEFAULT OUTPUT
################################################################## */

altmdi.build = function(data) {
	var outputHTML = '';
	$.each(data.results, function(i, result) {
		outputHTML += '<li class="item ' + result.type + ' ' + result.governmentClass + '">';
		if (result.url) outputHTML += '<a href="' + result.url + '">';
		if (result.name) outputHTML += '<h4 class="title">' + result.name + '</h4>';
		if (result.agencies[0]) outputHTML += '<span class="agency">' + result.agencies[0] + '</span>';
		//if (result.imageName) outputHTML += '<img src="/masterindex/index-images/' + result.imageName + '" title="' + result.name + '" alt="' + result.name + '" />';
		if (result.description) outputHTML += '<span class="description">' + result.description + '</span>';
		if (result.url) outputHTML += '</a>';
		outputHTML += '</li>';
	});
	return outputHTML
}

/* ##################################################################
	UTAH CITIES OUTPUT /city/list.html
################################################################## */

altmdi.buildCitiesList = function(data) {
	var outputHTML = '';
	$.each(data.results, function(i, result) {
		var urlfix = result.name.replace(/\s+/g, '+');
		//var urlfix = encodeURI(result.name);
		console.log(urlfix);
		outputHTML += '<li class="item ' + result.type + '">';
		outputHTML += '<a href="/city/city.html?view=' + urlfix + '">';
		outputHTML += result.name;
		outputHTML += '</a>';
		outputHTML += '</li>';
	});
	return outputHTML
}


/* ##################################################################
	BACKGROUND OUTPUT (FOR LOCALS SECTION/PAGE)
################################################################## */

altmdi.outputBackground = function(data, args) {
	//masterindex/index-images/8825.png EXAMPLE IMG PATH
	// console.log(args);
	var resultCount = data.totalCount;
	if (resultCount != 0) {
		background = '/masterindex/index-images/' + data.results[0].imageName;
		//console.log(background);
		$(args.mdiContainer).css('background-image', 'url("' + background + '")');
	} else {
		console.log('no background found');
	}
	
}

/* ##################################################################
	OUTPUT
################################################################## */

altmdi.output = function(data, args) {
	var outputHTML = '';
	if (!args.format) args.format = 'default';
	if (args.counterHTML) outputHTML += args.counterHTML;

	if (args.format === 'optionlist') {
		outputHTML += args.HTML;
	}
	else {
		if (args.HTML) {
			if (args.content) outputHTML += '<span class="data-content">' + args.content + '</span>';
			outputHTML += '<ul class="MDI-Results ' + args.format + '">';
			outputHTML += args.HTML;
			outputHTML += '</ul>';
			if (args.getmoreHTML) outputHTML += args.getmoreHTML;
		}
		else if (!args.HTML) {
			outputHTML += 'No Results Found';
		}
	}
	
	// OUTPUT
	$(args.mdiContainer).html(outputHTML);
}



/* trigger when page is ready */
$(document).ready(function (){

	altmdi.getGeo();

});


/* optional triggers

$(window).load(function() {
	
});

$(window).resize(function() {
	
});

*/


})(window.jQuery);