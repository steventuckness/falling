using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class CloneOptions {
    public static CloneOption GetOption(ECloneOption opt) {
        switch (opt) {
            case ECloneOption.RED:
                return new CloneOption("Red", Color.Color8(255, 0, 0, 255));
            case ECloneOption.GREEN:
                return new CloneOption("Green", Color.Color8(0, 255, 0, 255));
            case ECloneOption.BLUE:
                return new CloneOption("Blue", Color.Color8(0, 0, 255, 255));
            default:
                return new CloneOption("Red", Color.Color8(255, 0, 0, 255));
        }
    }

    public static List<CloneOption> OptionsFrom(ECloneOption[] eClone) =>
        eClone.ToList().Select(e => CloneOptions.GetOption(e)).ToList();

    public enum ECloneOption {
        RED,
        GREEN,
        BLUE
    }

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
}
