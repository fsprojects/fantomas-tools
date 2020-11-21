import { promises as fs } from "fs";
import glob from "glob";

const folderExists = await (fs.access("./build").then(() => true));
if (folderExists) {
    await fs.writeFile("./build/.nojekyll", "");
    await fs.rmdir("./build/_dist_/FantomasTools", { recursive: true });
    glob("./build/_dist_/bin/**/*.{fs,fsproj,gitignore,txt}", { dot: true }, async (err, files) => {
        await Promise.all(
            files.map(file => {
                return fs.rm(file);
            }));
    });
}
