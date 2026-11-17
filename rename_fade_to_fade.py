import os


def rename_fade_to_fade():
    # Ermittelt den Ordner, in dem dieses Skript liegt
    root_dir = os.path.dirname(os.path.abspath(__file__))

    print(f"Starte Umbenennung im Ordner: {root_dir}\n")

    # 1. Dateien umbenennen
    for dirpath, dirnames, filenames in os.walk(root_dir):
        for filename in filenames:
            if "godot" in filename.lower():
                # Erstellt den neuen Dateinamen (behält die originale Groß-/Kleinschreibung für den Rest bei)
                new_filename = filename.replace("godot", "fade").replace(
                    "Godot", "FADE"
                )
                old_file_path = os.path.join(dirpath, filename)
                new_file_path = os.path.join(dirpath, new_filename)

                os.rename(old_file_path, new_file_path)
                print(
                    f"Datei umbenannt: {filename} -> {new_filename}"
                )

    # 2. Ordner umbenennen (von unten nach oben, um Pfadkonflikte zu vermeiden)
    for dirpath, dirnames, filenames in os.walk(root_dir, topdown=False):
        for dirname in dirnames:
            if "godot" in dirname.lower():
                new_dirname = dirname.replace("godot", "fade").replace(
                    "Godot", "FADE"
                )
                old_dir_path = os.path.join(dirpath, dirname)
                new_dir_path = os.path.join(dirpath, new_dirname)

                os.rename(old_dir_path, new_dir_path)
                print(f"Ordner umbenannt: {dirname} -> {new_dirname}")

    print("\nFertig!")


if __name__ == "__main__":
    # Sicherheitsabfrage vor der Ausführung
    bestaetigung = input(
        "Möchtest du alle 'godot' Dateien/Ordner in 'fade' umbenennen? (ja/nein): "
    )
    if bestaetigung.lower() in ["ja", "j"]:
        rename_fade_to_fade()
    else:
        print("Vorgang abgebrochen.")
