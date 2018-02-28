using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class CloneOptions  {
    private static CloneOptions options;
    
    public enum ECloneOption {
        RED,
        GREEN,
        BLUE
    }

    private Dictionary<ECloneOption, CloneOption> list = new Dictionary<ECloneOption, CloneOption>();

    public class CloneOption {
        private String name;
        private Color c;

        public Color GetColor() {
            return this.c;
        }

        public CloneOption(String name, Color c) {
            this.name = name;
            this.c = c;
        }
    }

    public CloneOptions() {
        // Create the dictionary of available clone options. Right now
        // this is just basic colors and will likely need to be updated to use
        // assets to support more advanced functionality.
        this.list.Add(ECloneOption.RED,     new CloneOption("Red", Color.Color8(255, 0, 0, 255)));
        this.list.Add(ECloneOption.GREEN,   new CloneOption("Green", Color.Color8(0, 255, 0, 255)));
        this.list.Add(ECloneOption.BLUE,    new CloneOption("Blue", Color.Color8(0, 0, 255, 255)));
    }

    public Dictionary<ECloneOption, CloneOption> GetOptions() => this.list;
    public List<CloneOption> GetAvailableOptions() => new List<CloneOption>(this.list.Values);
    public List<CloneOption> OptionsFrom(ECloneOption[] eClone) => eClone.ToList().Select(e => this.list[e]).ToList();

    public static CloneOptions Instance {
        get {
            if(options == null) {
                options = new CloneOptions();
            }
            return options;
        }
    }
}
