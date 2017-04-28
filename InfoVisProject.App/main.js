﻿


function init() {

	// Create the Google Map…				
	var map = new google.maps.Map(d3.select("#map").node(), {
		zoom: 10,
		center: new google.maps.LatLng(37.75, -122.35),
		mapTypeId: google.maps.MapTypeId.TERRAIN
	});


	$("#yearSlider").slider({
		range: true,
		min: 1977,
		max: 2014,
		values: [1977, 2014],
		step: 1,
		slide: onSlide
	});


	$("#amount").val("$" + $("#yearSlider").slider("values", 0) +
		" - $" + $("#yearSlider").slider("values", 1));


	// Create Slider

	// Load the station data. When the data comes back, create an overlay.
	d3.json("stations.json", function (error, data) {
		if (error) throw error;

		var overlay = new google.maps.OverlayView();

		// Add the container when the overlay is added to the map.
		overlay.onAdd = function () {
			var layer = d3.select(this.getPanes().overlayLayer).append("div")
				.attr("class", "stations");

			// Draw each marker as a separate SVG element.
			// We could use a single SVG, but what size would it have?
			overlay.draw = function () {
				var projection = this.getProjection(),
					padding = 10;

				var marker = layer.selectAll("svg")
					.data(d3.entries(data))
					.each(transform) // update existing markers
					.enter().append("svg")
					.each(transform)
					.attr("class", "marker");

				// Add a circle.
				marker.append("circle")
					.attr("r", 4.5)
					.attr("cx", padding)
					.attr("cy", padding);

				// Add a label.
				marker.append("text")
					.attr("x", padding + 7)
					.attr("y", padding)
					.attr("dy", ".31em")
					.text(function (d) { return d.key; });

				function transform(d) {
					d = new google.maps.LatLng(d.value[1], d.value[0]);
					d = projection.fromLatLngToDivPixel(d);
					return d3.select(this)
						.style("left", (d.x - padding) + "px")
						.style("top", (d.y - padding) + "px");
				}
			};
		};

		// Bind our overlay to the map…
		overlay.setMap(map);
	});
}







function onSlide(event, ui) {
	$("#amount").val("$" + ui.values[0] + " - $" + ui.values[1]);

	// TODO CHANGE VISUALIZATION
}
