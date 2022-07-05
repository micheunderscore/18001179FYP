using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour {
    public List<string> debug = new List<string>();
    public List<float> meanBank = new List<float>();

    public void update(string id, string msg) {
        int index = debug.FindIndex(a => a.Substring(0, 2).Contains(id));
        if (index != -1) {
            debug[int.Parse(id)] = $"{id} - {msg}";
        } else {
            debug.Add($"{id} - {msg}");
        }
    }

    public void FixedUpdate() {
        update("00", $"Mean Results = ({meanBank.Count}) {Queryable.Average(meanBank.AsQueryable())}");
        meanBank.Clear();
    }

    public void OnGUI() {
        GUI.Label(
            new Rect(
                0,          // x, left offset
                50,         // y, bottom offset
                250f,       // width
                500f        // height
            ),
            string.Join("\n", debug),    // the display text
            GUI.skin.textArea            // use a multi-line text area
        );
    }
}
