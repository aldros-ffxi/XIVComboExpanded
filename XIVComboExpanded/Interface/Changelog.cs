using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboExpanded.Interface
{
    public class Changelog
    {
        public static Dictionary<string, string[]> GetChangelog()
        {
            return new Dictionary<string, string[]>()
                {
                    {
                        "v2.0.0.0",
                        ["Initial re-release!",
                        "Every job is Dawntrail updated at least up to the level 90.",
                        "Please request reasonable new features on GitHub (link in the About tab) if you'd like to see new combos.",
                        "A one-time pop-up allows for an easier first installation. It can be re-enabled in the Settings tab.",
                        "Please note that some jobs do not have any combos available at all if you don't enable Expanded combos.",]
                    },
                };
        }
    }
}
