/*
 * MavpixelGUI - Front end for Mavpixel Mavlink Neopixel bridge
 * (c) 2016 Nick Metcalfe
 *
 * Mavpixel is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Please read licensing, redistribution, modifying, authors and 
 * version numbering from main sketch file. This file contains only
 * a minimal header.
 *
 * MavpixelGUI is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Mavpixel.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml;
using System.Reflection;

namespace MavpixelGUI
{
    class UpdateWorker
    {
        public BackgroundWorker worker = new BackgroundWorker();
        string name = "";
        public UpdateWorker(string programName)
        {
            worker.WorkerReportsProgress = true;
            name = programName;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }
        
        public void checkForUpdate(string url)
        {
            worker.RunWorkerAsync(url);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Version newVersion = null;
            string downloadUrl = "";
            string notes = null;
            XmlTextReader reader = null;
            try
            {
                worker.ReportProgress(0, null);
                reader = new XmlTextReader((string)e.Argument);
                worker.ReportProgress(25, null);
                reader.MoveToContent();
                worker.ReportProgress(50, null);
                string elementName = "";
                if ((reader.NodeType == XmlNodeType.Element) &&
                    (reader.Name == "MavpixelGUI"))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                            elementName = reader.Name;
                        else
                        {
                            if ((reader.NodeType == XmlNodeType.Text) &&
                                (reader.HasValue))
                            {
                                // we check what the name of the node was  
                                switch (elementName)
                                {
                                    case "version":
                                        newVersion = new Version(reader.Value);
                                        break;
                                    case "url":
                                        downloadUrl = reader.Value;
                                        break;
                                    case "notes":
                                        notes = reader.Value;
                                        break;
                                }
                            }
                        }
                    }
                }
                worker.ReportProgress(75, null);
                if (newVersion == null) throw new Exception("Version information not found in CurrentVersion.xml!");
                if (Assembly.GetExecutingAssembly().GetName().Version < newVersion)
                {
                    string result = @"{\rtf1\ansi\qc\b\fs20" + imageRtf + @" A newer version of " + name + " (v" + newVersion.ToString() + @") is available.\b0\par\ql" +
                        horizontalBarRtf + @"\fs16\par Please visit " + downloadUrl + @" to download the latest version from SourceForge.\par";
                    if (notes != null) result += @"\par\b Notes for v" + newVersion.ToString() + @":\b0 " + notes.Replace("\n", @"\par ") + @"\par}";
                    else result += "}";
                    e.Result = result;
                }
                else e.Result = @"{\rtf1\ansi" + tickRtf + " " + name + @" is up to date.\par}";
            }
            catch (Exception ex)
            {
                e.Result = @"{\rtf1\ansi Error during update check:\par " + ex.Message + @"\par}";
            }
            finally
            {
                if (reader != null) reader.Close();
            }

        }


        static public string horizontalBarRtf = @"{\pict\wmetafile8\picw12777\pich117\picwgoal7245\pichgoal60 0100090000035b00000004000800000000000400000003010800050000000b0200000000050000000c022100280e030000001e0008000000fa0200000300000000008000040000002d01000007000000fc020100000000000000040000002d010100080000002503020011001100170e110008000000fa0200000000000000000000040000002d01020007000000fc020000ffffff000000040000002d01030004000000f0010000040000002701ffff030000000000}";
        static public string tickRtf = @"{\pict\wmetafile8\picw423\pich423\picwgoal240\pichgoal240 
010009000003ca0200000000a102000000000400000003010800050000000b0200000000050000
000c0210001000030000001e0004000000070104000400000007010400a1020000410b2000cc00
100010000000000010001000000000002800000010000000100000000100080000000000000000
000000000000000000000000000000000000000000ffffff009aca9f00a5cea90063b36d005faf
6900a5cba90099c99e0062b26c0082d18f007ac8850057a660009fc4a20099c89e0060b06a0081
cf8d007fcf8b0058a7610039854000f9fbf900d5edd800bee2c30099c89d007fce8a007ece8900
39833f00d9efdc006cbd75006dc07900b5dbb90098c79d005eae68007dcd89007ccd870056a55f
0038823e00d3ecd6006cbd760079c9860080ce8d0053a75c00b2d6b5009cc9a0005cad67007ccc
860079cb850054a45d0037813d00f9fcfa0059b063006bbd760084d290007ac9850060b26a0063
b46d0078c9830078cb820053a35c0036803c00f8fcf90051a65a0063b56d007bcc870076ca8100
76c9810052a25a00357f3b00f8fbf800499a51005bac640077ca820074c87e0051a05900347e3a
00f8fbf900408e470054a35c004f9f5700337d3900f7faf80037833d00347d3a00f7faf7000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000000000000000000000000000
000000000000000000000000000000000000000000000000000000010101010101010101010101
0101010101010101015213010101010101010101010101014f5051130101010101010101010101
434b4c4d4e4a010101010101010101434445464748494a010101010101013b3c3d183e3f404142
130101010101303132333435363738393a13010101012425262728292a2b2c2d2e2f1301010101
1a1b1c1d01011e1f202122231301010101141501010101160517180b1913010101010101010101
010d0e0f10111213010101010101010101010708090a0b0c010101010101010101010102040506
010101010101010101010101010203010101010101010101010101010101010101010101010101
01010101010101010101040000002701ffff030000000000
}";
        static public string imageRtf = @"{\pict\wmetafile8\picw794\pich767\picwgoal450\pichgoal435 
0100090000038005000000005705000000000400000003010800050000000b0200000000050000
000c021d001e00030000001e000400000007010400040000000701040057050000410b2000cc00
1d001e00000000001d001e0000000000280000001e0000001d00000001001800000000006c0a00
0000000000000000000000000000000000ffffffffffffffffffffffffffffffffffffffffffff
ffffffffffffffffdee7e7a5bdb584ad9c84bda57bb59c7bad9c94b5addee7e7ffffffffffffff
ffffffffffffffffffffffffffffffffffffffffffffffffffffffffff0000ffffffffffffffff
ffffffffffffffffffffffffffffffffffffffbddecea5d6bdb5e7cebdefd6ade7cea5e7ce9cde
bd94debd7bc6a5a5d6bddee7e7ffffffffffffffffffffffffffffffffffffffffffffffffffff
ffffffff0000fffffffffffffffffffffffffffffffffffff7f7f7d6d6d67bad94addec6bdefd6
bdefd6b5e7ceade7ce9cdebd9ce7bd94debd94debd8cd6ad6bad949cada5e7e7e7efefeff7f7f7
ffffffffffffffffffffffffffffffffffff0000ffffffa5cebdf7fff7fffffffffffff7f7f7d6
d6d67ba594b5e7cebde7cebdefd6c6efd6c6efd6b5e7ceade7ce8cc6ad84bd9c7bb59c84c6a594
d6b584c6a594a59ce7e7e7e7e7e7f7f7f7f7f7f7ffffffffffffffffffffffff0000ffffff8cc6
ad94cead9cc6b5b5c6bde7e7e784a594addec6ade7c6b5e7cebde7d6c6efdec6efd6add6c684a5
849494739c8c6b9c8c6b84846b7b9c7b84bd9c73b594adb5adf7f7f7f7f7f7ffffffffffffffff
ffffffffffffff0000ffffffbddece9cdebd94deb58ccead73ad9494ceb5ade7c6ade7ceb5e7ce
bdefd6c6efdeadd6bd8c9c73d6c6a5d6b59cbd946bad8c63ad8c63a584639494736ba58463a58c
eff7f7ffffffffffffffffffffffffffffffffffff0000ffffffeff7f784c6a594deb594d6b59c
debda5debda5e7c6ade7c6b5e7cebde7ceb5dec68c946bc69c6bcead7bcead7bc69c6bc69c6bb5
8c5ab58c63ad8463a5947b4a845ac6ded6ffffffffffffffffffffffffffffffffffff0000ffff
ffffffff94ceb594debd94deb594deb59cdebda5debdade7ceb5e7cebde7d684a57bc6a56bd6ad
73d6a56bcea56bd6ad73cea56bc69463bd8c5abd9463bd9c7b9484636b9c7bffffffffffffffff
ffffffffffffffffffff0000ffffffffffffb5d6ce94d6b58cd6b594deb594deb5a5e7c6a5dec6
b5e7ceb5dece8cb594adb594ceb584d6ad73d6ad73d6a56bd6ad73ce9c63c6945abd8c5ac6a57b
a584637b7b5ad6e7deffffffffffffffffffffffffffffff0000ffffffffffffeff7f78ccead94
debd94d6b59cdebd9cdebdade7c6ade7c6bdefd6bdefd6addec694b594a5ad84ceb584deb584d6
b57bdead73ce9c5ac6945acea573b5946b9c7b5aada594ffffffffffffffffffffffffffffff00
00ffffffffffffffffff8cc6ad94deb594deb594d6b59cdebda5dec6b5e7ceb5e7cec6efd6c6e7
d6b5dece7bb594739c73c6b584debd8cdebd8cd6a563c6944acea573bd8c5aa5845a94735afff7
f7ffffffffffffffffffffffff0000ffffffffffffffffffadd6c69cdebd94deb59cdebda5dec6
ade7c6a5debd9cd6b594bda59cbd9ca5b594bdbd94cebd94debd94d6b584debd94d6b584deb57b
deb58cc6945aad845a94734aded6ceffffffffffffffffffffffff0000ffffffffffffffffffe7
f7ef84c6ad94d6b58cc6a58cb58c8cad8ca5b594b5b594cec69cd6c69cdecea5dec6a5decea5d6
bd9cdebd9cd6bd8cd6b584debd94e7cea5c6945abd9463946b42cebdb5ffffffffffffffffffff
ffff0000ffffffffffffffffffffffff7394738c946ba58c52b5944acead73dec6a5e7ceaddece
ade7ceb5d6c69cdec6a5d6bd9cdec6a5dec6a5dec69cd6bd8cdebd8cd6ad73d6a56bb594638c73
4aa5a594ffffffffffffffffffffffff0000ffffffffffffffffffffffff9c8463bd8c5ac69452
ce9c52cea563dec694dec6a5deceadd6c6a5d6c6a5d6c6a5deceaddeceade7ceb5d6c69cd6bd94
c6bd94adad7b8c9c7384ad847bb5948cbda5ffffffffffffffffffffffff0000ffffffffffffff
ffffffffffad947bad7b42cea563cea563cead73d6bd94e7d6b5deceb5e7d6b5e7d6bdefdec6de
ceb5cec6a5b5bd9c9cb59494b59494c6ad94ceb59cdebd9cdebd9ce7bd8cc6adf7fff7ffffffff
ffffffffff0000ffffffffffffffffffffffffbdad94a57342c68c4ad6ad6bcead7bd6bd94d6c6
a5e7d6bdefdececed6bd94bd9c9cc6a59cc6adadd6c6b5deceb5e7ceade7c6ade7c69cdebd94de
bd8cd6b594d6b5c6ded6ffffffffffffffffff0000ffffffffffffffffffffffffe7ded6946b39
c68c4ace9c5ad6b57bd6c69cefe7ceefe7d6efe7ced6dec69cbda58cc6adb5e7cec6efdec6efd6
b5e7ceb5e7cea5dec6a5e7c694deb594deb594deb59cceb5ffffffffffffffffff0000ffffffff
ffffffffffffffffffffff8c8463b58442ce9452cea56bd6bd94e7dec6f7efe7efdec6f7efd6ef
e7d6d6dec6a5c6a59cceb5b5dec6bde7d6ade7c6ade7c69cdebd9cdebd8cd6b594debd84c6a5f7
fff7ffffffffffff0000ffffffffffffffffffffffffffffffc6d6ce8c7b4ac69452cead6bceb5
8cdec6a5e7debdf7e7cef7efd6fff7e7f7efdef7efd6ced6bd8cc6adbde7d6b5e7ceade7c6a5e7
c69cdebd94debd94deb594d6b5c6ded6ffffffffffff0000ffffffffffffffffffffffffffffff
ffffff5a9473b5a57bd6ad7bd6b584d6c6a5efdec6efe7cef7efd6f7efd6f7efd6e7dec6b5ceb5
add6c6bdefd6ade7c6ade7c6a5debda5debd94d6b594deb594d6b59cceb5ffffffffffff0000ff
ffffffffffffffffffffffffffffffffffc6e7d6529c73b5b58cd6b584dec6a5e7dec6f7efdeff
f7defff7deefe7c6b5ceada5ceb5ceefdebde7d6b5e7ceade7c6ade7ce9cd6bd94d6b594deb59c
e7bd8cc6adffffffffffff0000ffffffffffffffffffffffffffffffffffffffffff8cc6ad73ad
8c94a573c6ad7bd6bd94e7ceb5efdec6d6d6b5a5bda59cceb5c6efd6c6e7d6bdefd6b5e7ceb5e7
cea5debd9cceb5c6ded694ceb58ccead94d6b5cee7deffffff0000ffffffffffffffffffffffff
fffffffffffffffffff7fff784c6ad8ccead84bd948cad849cad8494ad848cbda59cd6bdbdefd6
bdefd6c6efdebde7d6bdefd6b5e7ce94c6adf7ffffffffffffffffdeefef94ceb594ceb5ffffff
0000ffffffffffffffffffffffffffffffffffffffffffffffffe7efef84c6a58cd6b58cd6b584
cead94d6b59cdebda5e7c6ade7c6b5e7cebde7d6bdefd6b5e7ce94c6b5eff7f7ffffffffffffff
ffffffffffffffffe7efefffffff0000ffffffffffffffffffffffffffffffffffffffffffffff
fffffffff7fff79cceb573c6a58cdeb58cdeb594debd9cdebdade7ceb5e7cec6efdeaddec69cce
b5eff7f7ffffffffffffffffffffffffffffffffffffffffffffffff0000ffffffffffffffffff
ffffffffffffffffffffffffffffffffffffffffffffffffd6efe794c6ad84c6ad84cead94d6b5
94d6b59cd6bd9cceb5c6e7d6ffffffffffffffffffffffffffffffffffffffffffffffffffffff
ffffff0000ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff
fffffffffff7fff7e7f7efdeefe7e7f7efeff7f7ffffffffffffffffffffffffffffffffffffff
ffffffffffffffffffffffffffffffffff0000ffffffffffffffffffffffffffffffffffffffff
ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff
ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff0000040000002701
ffff030000000000
}";

    }
}
