var mainModel = {rows:[]};
var commModel = {rows:[]};
function handleFeed(result, model, type){
    if (!result.error){
        for (var i = 0; i < result.feed.entries.length; i++) {
            var entry = result.feed.entries[i];
            var row = {feed: result.feed, title: entry.title, link: entry.link, author: entry.author, datestr: entry.publishedDate, visible:true, date: new Date(entry.publishedDate)};
            model.rows[model.rows.length] = row;
        }
        if (type == "main"){
            renderMain();
        }else{
            renderComm();
        }
    }
}
function clickHandler(item){
    mainModel.rows[item].visible = false;
    renderMain();
}
function sort(i1,i2){ return i2.date.getTime() - i1.date.getTime();}
function initialize() {
    for (i = 0; i < feeds.length; i++){
        feeds[i].hmain = new google.feeds.Feed(feeds[i].rss);

        if (feeds[i].comments != ""){
            feeds[i].hcomm = new google.feeds.Feed(feeds[i].comments);
        }

    }
    //feedModel.feeds = feeds;
    for (i = 0; i < feeds.length; i++){
        feeds[i].hmain.load(function(result) { handleFeed(result, mainModel, "main"); });
        if (feeds[i].hcomm != 0){
            feeds[i].hcomm.load(function(result) { handleFeed(result, commModel, "comm"); });
        }
    }
}
function twodigit(val){
    if (val < 10){
        return "0" + val;
    }else{
        return val;
    }
}
function renderComm(){
    var model = commModel.rows.sort(sort);

    var main = document.getElementById("comments");
    var body = document.getElementById("comments-body");
    //main.removeChild(body);

    body = document.createElement("ul");
    body.setAttribute("id", "comments-body");
    
    for (i = 0; i < (model.length > 10 ? 10:model.length); i++){
        var item = model[i];

        var li = document.createElement("li");
        var p = document.createElement("p");
        var br = document.createElement("br");
        var anchor1 = document.createElement("a");
        anchor1.setAttribute("href", item.link);
        anchor1.setAttribute("target", "new");
        anchor1.appendChild(document.createTextNode(item.title));

        p.appendChild(anchor1);
        p.appendChild(br);


        var anchor2 = document.createElement("a");
        anchor2.setAttribute("href", item.feed.link);
        anchor2.setAttribute("target", "new");
        anchor2.appendChild(document.createTextNode(item.feed.title));

        p.appendChild(anchor2);
        li.appendChild(p);
        body.appendChild(li);
    }

    //main.appendChild(body);
}
function frmdt(dt){
    var dobj = new Date(dt);
    return  dobj.getFullYear() + "-" +
            twodigit(dobj.getMonth() + 1) + "-" +
            twodigit(dobj.getDate()) + " " +
            twodigit(dobj.getHours()) + ":" +
            twodigit(dobj.getMinutes()) + ":" +
            twodigit(dobj.getSeconds());
}

function renderMain(){
    var model = mainModel.rows.sort(sort);
    
    var tbl = document.getElementById("feed-table");
    var body = document.getElementById("feed-body");
    tbl.removeChild(body);

    body = document.createElement("tbody");
    body.setAttribute("id", "feed-body");

    for (i = 0; i < model.length; i++){
        var item = model[i];
        if (item.visible){
            var feed = item.feed;
            var tr = document.createElement("tr");
			var td0 = document.createElement("td");
			var icon = document.createElement("img");
            icon.setAttribute("src", "/images/icons/feeds.svg");
			td0.appendChild (icon);
			
            var td1 = document.createElement("td");
            td1.setAttribute("class", "blogTitle");
            var anchor1 = document.createElement("a");
            anchor1.setAttribute("href", item.link);
            anchor1.setAttribute("target", "new");
            anchor1.appendChild(document.createTextNode(item.title));
            td1.appendChild(anchor1);
            td1.appendChild(document.createElement("br"));
            var anchor2 = document.createElement("a");
            anchor2.setAttribute("href", feed.link);
            anchor2.setAttribute("target", "new");
            anchor2.appendChild(document.createTextNode(feed.title));
            td1.appendChild(anchor2);
            var td2 = document.createElement("td");
            td2.appendChild(document.createTextNode(frmdt(item.datestr)));
            var td3 = document.createElement("td");
            var anchor3 = document.createElement("a");
            anchor3.setAttribute("href", "#");
			anchor3.setAttribute("title", "Remove Blog Post");
            anchor3.setAttribute("onClick", "clickHandler(" + i + ");");
			anchor3.setAttribute("id", "remove");
            var img = document.createElement("img");
            img.setAttribute("src", "/images/icons/clearSearch.png");
            anchor3.appendChild(img);
            td3.appendChild(anchor3);
            tr.appendChild(td0);
			tr.appendChild(td1);
            tr.appendChild(td2);
            tr.appendChild(td3);
            body.appendChild(tr);
        }
    }
    tbl.appendChild(body);
}

// function renderMain() {

// 	var model = mainModel.rows.sort(sort);
		
// 	var ul = document.getElementById("blogFeed");

// 	for (i = 0; i < model.length; i++) {

// 		var item = model[i];

// 		if (item.visible) {

// 			var feed = item.feed;

// 			var li = document.createElement("li");
// 				li.setAttribute("class", "item Blog");

// 			var anchor = document.createElement("a");
// 				anchor.setAttribute("href", item.link);
// 				anchor.setAttribute("target", "new");
		
// 			var h4 = document.createElement("h4");
// 				h4.setAttribute("class", "title");
// 				h4.appendChild(document.createTextNode(item.title));
				
// 			var spanTitle = document.createElement("span");
// 				spanTitle.setAttribute("class", "description");
// 				spanTitle.appendChild(document.createTextNode(feed.title));

// 			var spanDate = document.createElement("span");
// 				spanDate.setAttribute("class", "date");
// 				spanDate.appendChild(document.createTextNode(frmdt(item.datestr)));

// 			anchor.appendChild(h4);
// 			anchor.appendChild(spanTitle);
// 			anchor.appendChild(spanDate);
// 			li.appendChild(anchor);
// 		}
// 	}

// 	ul.appendChild(li);
// }