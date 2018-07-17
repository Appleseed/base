/*
	Template script for processing a CSV file and pushing results to a Solr endpoint.
	The CSV gets converted to a JSON file for processing and data manipulation.
*/

"use strict";

var DataTransform = require("node-json-transform").DataTransform;
var http = require('http');
var https = require('https');
var download = require('download');
var crypto = require('crypto');
var exec = require('child_process').exec;
var fsPath = require('fs-path');
var Promise = require('bluebird');
var fs = Promise.promisifyAll(require('fs'));
var xlsxj = require("xlsx-to-json");
var solr = require('solr-client');
var csvjson = require('csvjson');

/**** Start User-defined variables ****/

// Define Solr client endpoint
var client = solr.createClient({
    host: 'localhost',
    port: '8983',
	core: 'test-core',
	path: '/solr',
	secure: false // false for http; true for https
});  

// sourceDirectory must be the subdirectory this script is located.
var sourceDirectory = "template_source_csv";

// The URL to download the file from
var sourceDownloadLink = "http://website.com/SAMPLE_FILENAME.csv";

// Option to unzip source file.  Set to true if source file is a .zip.
var fileUnzip = false;

// The absolute filename of a download.  Set if download has a static file name.
//  If download is a .zip, this should be the extracted file name. 
//  Set to "" or null if filename may change.  Should be used if fileRegex is not needed
var fileName = "SAMPLE_FILENAME.csv";

// A regex used to match filenames.  Useful for download links that stay consistent but the filename changes,
//  or downloads that include multiple files (such as a compressed file that is extracted)
//
//  Inside 1st set of parenthesis: the partial file name
//  Inside 2nd set of parenthesis: the file extension
var fileRegex = /^(SAMPLE_FILENAME).*(\.csv)$/i;

// itemMap should be user-customized to map Solr fields (left) to JSON data fields (right)
var itemMap = {
	item: {
		id: "",
		path: "",
		item_type: "",
		content_type: "",

		field_1: "Example String",
		date_1: "Example Date",
		date_last_indexed: "",		
	},
	// operate transforms/manipulates/populates fields defined in itemMap.  Operations should be user customized.
	operate: [
		{
			// Solr can handle autogenerated ID numbers, but if you want you can map the Solr id to a field from
			//  the source JSON.  You can also use the anonymous function below to turn the string to an MD5 hash,
			//  or customize the function to create the hash from an array of values.
			// run: function (string) { return crypto.createHash('md5').update(string).digest("hex"); }, on: "id"
		},
		{
			// path will be set to the souceDownloadLink
			run: function () { return sourceDownloadLink }, on: "path"
		},
		{
			// item_type is set to the returned string.  Useful for categorizing added objects.
			run: function () { return "Sample Json Item" }, on: "item_type"
		},
		{
			// Setting content_type: parentDocument is for Solr parent-child document relationships
			run: function () { return "parentDocument" }, on: "content_type"
		},
		{
			// date_1 is transformed to be an ISO datestring format, which is accepted by Solr.
			run: prepDate, on: "date_1"
		},
		{
			// date_last_indexed is the date the object was last updated in Solr, i.e. when this script was run.
			//  If the file download contains objects that already exist in Solr, they will be compared by ID.
			run: function() { return new Date().toISOString()}, on: "date_last_indexed"
		}
	]
};

/**** End user-defined variables ****/

client.autoCommit = true;

function source() {
	this.FileName = "";
	this.DownloadLink = "";
	this.Mapping = "";
	this.FileNameJson = "";
};

var sourceItem = new source();
sourceItem.FileName = fileName;
sourceItem.DownloadLink = sourceDownloadLink;
sourceItem.Mapping = itemMap;
sourceItem.FileNameJson = fileName + ".json";

function prepDate(date) {
	if (date != undefined) {
		return new Date(date).toISOString();
	} else {
		return "";
	}
}

console.log(sourceDirectory + ": " + "Downloading...");

download(sourceItem.DownloadLink, sourceDirectory + "/receive", { extract: fileUnzip })
	.then(() => {
		//  sets sourceItem.FileName and sourceItem.JSONFileName
		fs.readdirAsync(sourceDirectory + "/receive").map(function(file) {
			if (sourceItem.FileName == "") {
				if (file.match(fileRegex)) {
					sourceItem.FileName = file;
					sourceItem.FileNameJson = sourceItem.FileName + ".json";
					console.log(sourceDirectory + ": " + "File Name: " + sourceItem.FileName);
					console.log(sourceDirectory + ": " + "Json File Name: " + sourceItem.FileNameJson);
					return;
				}
			}
			else {
				sourceItem.FileNameJson = sourceItem.FileName + ".json";
			}
		})	
		// After downloading, reads the data from sourceItem.JSONFileName and writes to a new file, sourceItem.FileName
		.then(() => {
			fs.readFileAsync(sourceDirectory + "/receive/" + sourceItem.FileName, { encoding : 'utf8'})
			.then((data) => {
				fsPath.writeFileSync(sourceDirectory + "/receive/" + sourceItem.FileNameJson, JSON.stringify(csvjson.toObject(data, {quote:'"'}), null, 1));
			})
			.then(() => {
				convertJson(sourceItem);
			});
		});
		
	});

function convertJson(item) {
	console.log(sourceDirectory + ": " + "Converting...");
	fs.readFileAsync(sourceDirectory + "/receive/" + item.FileNameJson, "utf8")
 	.then((data) => {
		if (item.Mapping != null && item.Mapping != "") {
			var result = DataTransform(JSON.parse(data), item.Mapping).transform();
			fsPath.writeFileSync(sourceDirectory + "/transform/results_" + item.FileNameJson, JSON.stringify(result, null, 1));
		} else {
			fsPath.writeFileSync(sourceDirectory + "/transform/results_" + item.FileNameJson, data);
		}
	}) 
	.then(() => {transmitJson(sourceItem)})
	.catch((err) => {
		console.log(sourceDirectory + ": " + err);
	});
};

function transmitJson(item) {
	fs.readFileAsync(sourceDirectory + "/transform/results_" + item.FileNameJson, "utf8")
	.then((data) => {
		console.log(sourceDirectory + ": " + "Transmitting...");
		var jsonData = JSON.parse(data);
		var text = "";
		client.add(jsonData, function(err, result) {
			if (err) {
				text = "Add error: \n" + err;
				console.log(sourceDirectory + ": " + text);
				fsPath.writeFile(sourceDirectory + "/transmit/log_" + item.FileNameJson + ".txt", text);
				return;
			}
			text = "Solr Add Response:\n" + JSON.stringify(result, null, 1);
			// Commit your changes without options
			client.commit(function(err,res){
				console.log(sourceDirectory + ": " + "Committing changes...");
				if(err) {
					text = "Commit error: \n" + err;
					console.log(sourceDirectory + ": " + text);
				} else {
					text += "\nSolr Commit Response:\n" + JSON.stringify(res, null, 1);
				}
				console.log(sourceDirectory + ": " + text);
				fsPath.writeFile(sourceDirectory + "/transmit/log_" + item.FileNameJson + ".txt", text);
			})
		});
	});
}
