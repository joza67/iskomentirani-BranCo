"use strict";


function htmlEncode(s) {
	return s.replace(/&/g, '&amp;')
		.replace(/</g, '&lt;')
		.replace(/>/g, '&gt;')
		.replace(/'/g, '&#39;')
		.replace(/"/g, '&#34;');
}

var i = 1;
var connection = new signalR.HubConnectionBuilder().withUrl("/wanesyhub").build();
connection.on("WanesyNotify", function (id, eventCreationTime, alarmCount, locationId, guid, moveeEventFrameId) {
	if (typeof onChange === "function") {
		onChange(id, eventCreationTime, alarmCount, locationId, guid, moveeEventFrameId);
	}
});

async function start() {
	try {
		await connection.start();
		console.log("SignalR Connected.");
		$("#signalStatus").removeClass('bg-danger').addClass('bg-success');
		$("#signalRconnectionStatus").removeClass('text-danger').addClass('text-success').html('Connected to Live server');
		
		var color = 'blue';

	} catch (err) {
		console.log(err);
		$("#signalStatus").removeClass('bg-success').addClass('bg-danger');
		$("#signalRconnectionStatus").removeClass('text-success').addClass('text-danger').html('Not connected');
		setTimeout(start, 5000);
	}
};

connection.onclose(start);

// Start the connection.
start();