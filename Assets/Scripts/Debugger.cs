using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour {
    public List<string> debug = new List<string>();
    public List<float> meanBank1 = new List<float>();
    public List<float> meanBank2 = new List<float>();

    public void Start() {
        update("00", $"BLU Mean Rewards = (0) 0", true);
        update("01", $"RED Mean Rewards = (0) 0", true);
    }

    public void update(string id, string msg, bool byId = false) {
        int index = debug.FindIndex(a => a.Substring(0, 2).Contains(id));
        if (index != -1) {
            debug[int.Parse($"{(byId ? id : index)}")] = $"{id} - {msg}";
        } else {
            debug.Add($"{id} - {msg}");
        }
    }

    public void FixedUpdate() {
        update("00", $"BLU Mean Rewards = ({meanBank1.Count}) {Queryable.Average(meanBank1.AsQueryable())}", true);
        meanBank1.Clear();

        update("01", $"RED Mean Rewards = ({meanBank2.Count}) {Queryable.Average(meanBank2.AsQueryable())}", true);
        meanBank2.Clear();
    }

    public void OnGUI() {
        GUI.Label(
            new Rect(
                10,         // x, left offset
                50,         // y, bottom offset
                250f,       // width
                500f        // height
            ),
            string.Join("\n", debug),    // the display text
            GUI.skin.textArea            // use a multi-line text area
        );
    }
}
