// remap jQuery to $
(function($){


//CUSTOM SEARCH.JS 

var srch = {};

srch.getUrlParam = function(param) {
    var results = new RegExp('[\?&]' + param + '=([^&#]*)').exec(window.location.href);
    if (results==null){
        return null;
    }
    else{
        return results[1] || 0;
    }
}


srch.results 		= $('#searchResults');
srch.resultcts 		= $('#results > div');
srch.tabslinks 		= $('#searchTabs li a');
srch.servicesList 	= $('#servicesList');
srch.formsResults 	= $('#formsResults');
srch.googleResults 	= $('#googleResults');
srch.relatedResults = $('#relatedResults');
srch.agencyList 	= $('#agency');
srch.trendingList 	= $('#trending');
srch.youtubeList 	= $('#youtube');
srch.keywordPre		= srch.getUrlParam('q');
srch.keyword		= srch.keywordPre.replace(/\+/g,' ');


/* ##################################################################
	SEARCH TABS
################################################################## */

srch.tabs = {
	open: function(tab, tabdata) {
		srch.tabs.closeall(); // Close All
		if (tabdata == 'servicesList') {
			srch.servicesList.addClass('search-panel-active');
		}
		if (tabdata == 'formsResults') {
			srch.cse.forms();
			srch.formsResults.addClass('search-panel-active');
		}
		if (tabdata == 'googleResults') {
			srch.cse.all();
			srch.googleResults.addClass('search-panel-active');
		}
		if (tabdata == 'relatedResults') {
			srch.relatedResults.addClass('search-panel-active');
		}

		tab.addClass('active');
	},
	close: function(tab, tabdata) {
		// console.log('close ' + tabdata);
		tab.removeClass('active');
	},
	closeall: function() {
		srch.tabslinks.removeClass('active');
		srch.servicesList.removeClass('search-panel-active');
		srch.formsResults.removeClass('search-panel-active');
		srch.googleResults.removeClass('search-panel-active');
		srch.relatedResults.removeClass('search-panel-active');
	}
}


/* ##################################################################
	GOOGLE CSE
################################################################## */


srch.cse = {

	forms: function() {
		srch.formsResults.html('<iframe scrolling="yes" frameborder="0" src="/gse/forms.html?q=' + srch.keyword + '"></iframe>');
	},
	all: function() {
		srch.googleResults.html('<iframe scrolling="yes" frameborder="0" src="/gse/all.html?q=' + srch.keyword + '"></iframe>');
	}
}




/* ##################################################################
	TIMER FOR DELAY RESULTS
################################################################## */

srch.delay = function(fn) {

	// CLEAR
	clearTimeout(srch.delaytimeout);
	// START NEW
	srch.delaytimeout = setTimeout(function(){
			
		if (fn == 'youtube') {
			// console.log('Calling YT');
			srch.output_youtube();
		}

	}, srch.delaytime);

}

/* ##################################################################
	OUTPUT SEARCH QUERY TEXT
################################################################## */

srch.output_querytext = function(get) {

	$('[data-getquery="true"]').each(function(i) {
		$(this).html(srch.keyword);
	});
}



/* ##################################################################
	QUERYMDI
################################################################## */

srch.get_services = function() {

	var args = {};
		args.agencymapsize 	= srch.agencyList.data('mapsize');
		args.type 			= srch.servicesList.data('type');
		args.keyword 		= srch.escout(srch.keyword);
		args.limit 			= srch.servicesList.data('limit');
		args.html 			= '';
		args.count			= '';
		args.agency			= '';

	$.getJSON(mdi.url, { keyword: args.keyword, typeIds: args.type, limit:args.limit }, function(data) {
		args.agency = data.topAgency;
		if (args.agency != null) { srch.output_topagency(data, args); }
		srch.output_trending(args);
		args.count = data.totalCount;
		// srch.output_services(data, args);
		
	});

	// TODO add .search-ajax-active class if results exist
}

/* ##################################################################
	OUTPUT SERVICES
################################################################## */

srch.output_services = function(data, args) {

	console.log('SECTION.JS');

	// args.html += '<ul class="my-new-list">';

	// if (args.count != 0) {

	// 	$.each(data.results, function(i, result) {

	// 		args.html += '<li class="item ' + result.type + ' ' + result.governmentClass + '">';
	// 		if (result.url) args.html += '<a href="' + result.url + '">';
	// 		if (result.name) args.html += '<h4 class="title">' + result.name + '</h4>';
	// 		if (result.agencies[0]) args.html += '<span class="agency">' + result.agencies[0] + '</span>';
	// 		//if (result.imageName) args.html += '<img src="/masterindex/index-images/' + result.imageName + '" title="' + result.name + '" alt="' + result.name + '" />';
	// 		if (result.description) args.html += '<span class="description">' + result.description + '</span>';
	// 		if (result.url) args.html += '</a>';
	// 		args.html += '</li>';

	// 	});

	// } else {
	// 	args.html += '<li class="item noresults">';
	// 	args.html += '<a href=#googleResults" class="getgoogleresults">';
	// 	args.html += '<h4 class="title">No Services results found with search term: "' + srch.keyword + '"</h4>';
	// 	args.html += '<span class="description">Try Searching "All Of Utah.gov"</span>';
	// 	args.html += '</a>';
	// 	args.html += '</li>';
	// }

	// args.html += '</ul>';

	// srch.servicesList.html(args.html);

}

/* ##################################################################
	OUTPUT AGENCY
################################################################## */

srch.output_topagency = function(data, args) {

	var agency = args.agency;

	if (agency) {

		var agencyhtml  = '<a href="https://maps.google.com/maps?f=q&amp;source=s_q&amp;hl=en&amp;geocode=&amp;q=' + agency.street1 + ' ' + agency.city + ' Utah ' + agency.zip + '" class="agency-map">'
			agencyhtml += '<img alt="' + agency.name + '" src="https://maps.google.com/maps/api/staticmap?center=' + agency.street1 + ' ' + agency.city + ' Utah ' + agency.zip + '&amp;zoom=13&amp;size=' + args.agencymapsize + 'x' + args.agencymapsize + '">';
			agencyhtml += '</a>';
			agencyhtml += '<h4>' + agency.name + '</h4>';
			agencyhtml += '<p class="agency-address">';
			agencyhtml += agency.street1 + ' ' + agency.street2 + '<br>'
			agencyhtml += agency.city + ', Utah ' + agency.zip + '<br>';
			agencyhtml += '<span class="agency-phone">' + agency.phone + '</span>';
			agencyhtml += '</p>';

		srch.agencyList.html(agencyhtml);
		srch.agencyList.addClass('active');

	} else {
		srch.agencyList.removeClass('active');
	}
}

/* ##################################################################
	OUTPUT TRENDING
################################################################## */

srch.output_trending = function(args) {

	$.getJSON(mdi.url, { keyword: args.keyword, typeIds: 54, limit: 1 }, function(data) {
		var result = data.results[0]; 
		if (result) {
			var trendinghtml = '<a href="' + result.url + '">' + result.description + '</a>';
			srch.trendingList.addClass('active');
		}
		else {
			var trendinghtml = '';
			srch.trendingList.removeClass('active');
		}
		
		srch.trendingList.html(trendinghtml);

	}); // $.getJSON

}

/* ##################################################################
	OUTPUT YOUTUBE
################################################################## */

srch.output_youtube = function() {

	//https://developers.google.com/youtube/v3/docs/search/list
	var ytContainer = document.getElementById('youtube');
	var ytWatchUrl = 'https://www.youtube.com/watch?v=';
	var ytPlaylistUrl = 'https://www.youtube.com/playlist?list=';
	var ytUrl = 'https://www.googleapis.com/youtube/v3/search';

	YTchannelId = 'UCkjGDFkwdRkRjyyykSkumew';
	YTapikey = 'AIzaSyDSUZ02zh9A4w5neKOb__JElQJRmtnPAUM';

	if (srch.escout(srch.keyword).length > 2) {
		$.getJSON(ytUrl, {part: 'snippet', q: srch.escout(srch.keyword), channelId: YTchannelId, maxResults: 3, key:YTapikey}, function(data) {

			var yhtml = '<ul>';

			for (var i = 0; i < data.items.length; i++) {
				// data.items[i]

				var ytWaId = data.items[i].id.videoId;
				var ytPlId = data.items[i].id.playlistId;

				if (ytWaId) {
					var ytLink = ytWatchUrl + '' + ytWaId;
				}
				else if (ytPlId) {
					var ytLink = ytPlaylistUrl + '' + ytPlId;
				}

				var ytTitle = data.items[i].snippet.title;
				// var ytDesc = data.items[i].snippet.description;
				var ytThumb = data.items[i].snippet.thumbnails.default.url;

				yhtml += '<li>';
				yhtml += '<a href="' + ytLink + '" target="_blank" style="background-image: url(' + ytThumb + ')" title="' + ytTitle + '">' + ytTitle + '</a>';
				yhtml += '</li>';
			}

			yhtml += '</ul>';
			ytContainer.innerHTML = yhtml;
		});	
	}

}


/* ##################################################################
	OUTPUT TWITTER
################################################################## */

srch.output_twitter = function(keyword) {

	var tweets = document.getElementById('tweets');
	var tweetsNo = document.getElementById('tweetsNo');

	if (srch.escout(srch.keyword).length > 2) {

		// var twitterUrl = 'https://demo.utah.gov/aggregator/tweetsForList.json';
		var twitterUrl = '/aggregator/tweetsForList.json';
		// https://demo.utah.gov/aggregator/tweetsForList.json?listOwner=UtahGov&slug=utah-government&keyword=test&count=10

		$.getJSON(twitterUrl, { listOwner: 'UtahGov', slug: 'utah-government', count: 5, keyword: srch.escout(srch.keyword) }, function(tweetData) {
			
			var twhtml = '';

			for (var i = 0; i < tweetData.length; i++) {
				var tweetUrl = tweetData[i].url;
				var tweetPost = tweetData[i].post;
				var tweetAuthor = tweetData[i].author;
				var tweetDate = tweetData[i].createdDate;
				// var tweetImg = tweetData[i].imageUrl;
				var tweetImg = false;


				if (tweetImg) {
					twhtml += '<a href="' + tweetUrl + '" title="' + tweetPost + '" target="_blank" class="tweetLink tweetHasImg" style="background-image: url(' + tweetImg + ');">';
				} else {
					twhtml += '<a href="' + tweetUrl + '" title="' + tweetPost + '" target="_blank" class="tweetLink">';	
				}
				
				twhtml += '	<span class="tweetPost">' + tweetPost + '</span>';
				twhtml += '	<span class="tweetAuthor">' + tweetAuthor + '</span>';
				twhtml += '	<span class="tweetDate">' + tweetDate + '</span>';
				// if (tweetImg) twhtml += '	<img src="' + tweetImg + '">';
				twhtml += '</a>';

			}

			tweets.innerHTML = twhtml;			

		});

	}

}

/* ##################################################################
	ESCAPE OUTPUT
################################################################## */

srch.escout = function(string) {
	var entitymap = {"&": "&amp;", "<": "&lt;", ">": "&gt;", "/": '&#x2F;'};
	return String(string).replace(/[&<>"'\/]/g, function (s) {
		return entitymap[s];
	});
}

/* ##################################################################
	KEYWORD DETECTION
################################################################## */

if (srch.keywordPre == 'hear+from+the+Governor') {
	$('.specialResults').removeClass('hidden');
	$('ul.herbert').removeClass('hidden');
	srch.keywordPre = 'herbert';
	srch.keyword = 'herbert';
	// window.location = 'https://www.utah.gov/stateofthestate/';
}
if (srch.keywordPre == 'See+the+State+Capitol') {
	window.location = 'https://www.utah.gov/capitol-tour/';
}
if (srch.keywordPre == 'Learn+about+changing+laws') {
	window.location = 'https://le.utah.gov/';
}
if (srch.keywordPre == 'experience+new+tech+from+Utah') {
	window.location = '/digital/';
}
if (srch.keywordPre == 'watch+the+inauguration') {
	window.location = '/governor/';
}

// if (srch.keywordPre == 'know+about+new+or+changing+laws') {
// 	$('.specialResults').removeClass('hidden');
// 	$('ul.le').removeClass('hidden');
// 	srch.keywordPre = 'legislation';	
// 	srch.keyword = 'legislation';
// }


//GET SERVICES RESULTS
// onlineservicesresults.data('keyword', searchKeyFix);
// onlineservicesresults[0].dataset.keyword = srch.keyword;
srch.servicesList[0].dataset.keyword = srch.keyword;
srch.formsResults[0].dataset.keyword = srch.keyword;
srch.googleResults[0].dataset.keyword = srch.keyword;

// console.log(srch.servicesList[0].dataset.keyword.length);
// console.log(srch.formsResults[0].dataset.keyword);
// console.log(srch.googleResults[0].dataset.keyword);
// onlineservicesresults.data('type', '1,2,3,5,63,6,48,65');

//GET SERVICES RESULTS
// servicesList.data('keyword', searchKeyFix);
// console.log(servicesList.data('keyword'));
// servicesList.data('type', '1,2,3,5,63,6,48,65');

/* trigger when page is ready */
$(document).ready(function (){
	
	// RUN MDI
	// mdi.initialize();
	srch.tabs.open($('#servicesTab a'), 'servicesList');
	srch.get_services();
	srch.output_querytext();
	srch.output_youtube();
	srch.output_twitter();

	// TAB SELECT
	srch.tabslinks.click(function(event) {
		// console.log(event);
		var tabdata = $(this).data('tab');
		// console.log(tabdata);
		if ($(this).hasClass('active')) { /* Already Open */ }
		else { srch.tabs.open($(this), tabdata); }
		event.preventDefault();
	});

	//getgoogleresults
	$('body').on('click', '.getgoogleresults',function(event){
		var tabdata = 'googleResults';
		srch.tabs.open($('[data-tab="googleResults"]'), tabdata);
		event.preventDefault();
	});

	//CSE
	(function() {
		//var cx = '011595275022494868614:jtle0ifunuw'; //TEST
		var cx = '005946968176299016736:b3muwlbpdj8'; //CURRENT ID
		var gcse = document.createElement('script');
		gcse.type = 'text/javascript';
		gcse.async = true;
		gcse.src = (document.location.protocol == 'https:' ? 'https:' : 'http:') +
		    '//www.google.com/cse/cse.js?cx=' + cx;
		var s = document.getElementsByTagName('script')[0];
		s.parentNode.insertBefore(gcse, s);
	})();



});


/* optional triggers

$(window).load(function() {
	
});

$(window).resize(function() {
	
});

*/


})(window.jQuery);