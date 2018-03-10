const rpc = require("rpc.js");
const Uri = require("vscode-uri").default;
const fs = require("fs");
const { URL } = require("url");
const highlight = require("highlight.js");

const apiSchema = {
	groups: {
		editor: {
			name: "Editor",
			info: "Some text document's methods"
		},
		math: {
			name: 'Math',
			info: 'Some example math methods',
		},
		util: {
			name: 'Utility calls',
			info: 'API test, debug and utility methods',
		}
	},

	methods: {
		FormatDocument: {
			info: "Format the JSON document",
			group: "editor",
			params: {
				uri: { required: true, type: "string", info: "The text document's uri" },
				languageId: { required: true, type: "string", info: "The text document's language identifier" },
				version: { required: true, type: "number", info: "The version number of this document (it will increase after each change, including undo/redo)" },
				text: { required: false, type: "string", info: "TBD, DO NOT use this param" }
			},
			action: function (params, output)
			{
				if (("" + params.text) != "")
				{
					output.fail(500, 'You can not use the param "text" for now');
				}
				// 先清除文本中所有空格、\r\n和\t
				// 每遇到一个{，就进行换行\r\n，并在制表符\t的数量上+1
				// 每一个冒号:后面都加一个空格
				// 每一个}都在新的一行，且制表符\t的数量-1
				let dest = "" + params.uri;
				fs.readFile(new URL(dest), (err, data) => {
					if (err) throw err;
					let content = new String(data.toString());
					console.log("初始JSON文本：\r\n" + content);
					content = content.replace(/\s+/gm, "").replace(/:/g, ": ");
					let result = Generate(content);
					console.log("\r\n格式化后JSON文本：\r\n" + result);
					output.win(result);
				});
			}
		},
		HighlightDocument: {
			info: "Color the JSON document",
			group: "editor",
			params: {
				uri: { required: true, type: "string", info: "The text document's uri" },
				languageId: { required: true, type: "string", info: "The text document's language identifier" },
				version: { required: true, type: "number", info: "The version number of this document (it will increase after each change, including undo/redo)" },
				text: { required: false, type: "string", info: "TBD, DO NOT use this param" }
			},
			action: function (params, output)
			{
				let color_attr = "<color=#E06C75FF>";
				let color_string = "<color=#98C379FF>";
				let color_number = "<color=#D19A66FF>";
				let color_literal = "<color=#32AAEEFF>";
				fs.readFile(new URL("" + params.uri), (err, data) => {
					if (err) throw err;
					let content = new String(data.toString());
					let dest = highlight.highlight("JSON", content, true).value.replace(/<\/span>/g, "</color>").replace(/<span class=\"hljs-attr\">/g, color_attr)
						.replace(/<span class=\"hljs-string\">/g, color_string).replace(/<span class=\"hljs-number\">/g, color_number).replace(/<span class=\"hljs-literal\">/g, color_literal);
					console.log(dest);
					output.win(dest);
				});
			}
		}
	}
};

function mainEntry()
{
	rpc.server("http", {
		port: 3000,
		address: "::",
		gateway: rpc.gateway({ schema: apiSchema })
	});
}

function Generate(src)
{
	let dest = new String("");
	let number_of_tabs = 0;

	for (const ch of src)
	{
		switch (ch)
		{
			case "{":
				number_of_tabs++;
				dest += (ch + "\r\n" + AddTabs(number_of_tabs));
				break;
			case "}":
				number_of_tabs--;
				dest += ("\r\n" + AddTabs(number_of_tabs) + ch);
				break;
			case ",":
				dest += (ch + "\r\n" + AddTabs(number_of_tabs));
				break;
			default:
				dest += ch;
				break;
		}
	}
	return dest;
}

function AddTabs(tabs)
{
	let str = "";
	for (let i = 0; i < tabs; i++)
	{
		str += "\t";
	}
	return str;
}

mainEntry();