const fs = require("fs");
const path = require("path");

function main()
{
	fs.copyFileSync(path.join(process.cwd(), ".build/help.html"), path.join(process.cwd(), "node_modules/rpc.js/ui/help.html"));
}

main();