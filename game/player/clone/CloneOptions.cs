using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class CloneOptions {
    public static CloneOption GetOption(ECloneOption opt) {
        switch (opt) {
            case ECloneOption.RED:
                return new CloneOption("Red", PlayerColor.Value.Red);
            case ECloneOption.GREEN:
                return new CloneOption("Green", PlayerColor.Value.Green);
            case ECloneOption.BLUE:
                return new CloneOption("Blue", PlayerColor.Value.Blue);
            default:
                return new CloneOption("Red", PlayerColor.Value.Red);
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
        private PlayerColor.Value c;

        public Color GetColor() {
            return PlayerColor.ToColor(this.c);
        }

        public PlayerColor.Value GetPlayerColor() {
            return this.c;
        }

        public CloneOption(String name, PlayerColor.Value c) {
            this.name = name;
            this.c = c;
        }
    }
}
