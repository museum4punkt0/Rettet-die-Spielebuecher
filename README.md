# Rettet die Spielebücher - Robo gegen die Spielefresser

### Inhaltsverzeichnis

- [Beschreibung](#Beschreibung)
- [Kurzbeschreibung](#Kurzbeschreibung)
- [Entstehungskontext](#Entstehungskontext)
- [Förderung](#Förderung)
- [Installation und Benutzung](#Installation-und-Benutzung)
- [Credits](#Credits)
- [Lizenz](#Lizenz)


# Beschreibung
Das Spiel "Rettet die Spielebücher - Robo gegen die Papierfresser" ist ein spielbarer Prototyp, der Kindern von sechs bis zehn Jahren anhand einfacher interaktiver Rätsel die materiellen Besonderheiten historischer Bewegungs- und Spielbilderbücher aus dem Bestand der Staatsbibliothek zu Berlin vermittelt. In insgesamt drei Leveln mit steigendem Schwierigkeitsgrad stehen die Kinder vor der Aufgabe, ihren Robotor durch das Labyrinth eines Aufstellbuchs zu steuern und verlorengegangene Teile aus verschiedenen Spielebüchern einzusammeln, die Bücher zu reparieren und schließlich die Papierfresser zu vertreiben. </br></br>
Das Spiel wurde mit Unity als WebGL Projekt erstellt und kann online in einem Browser ausgeführt werden. Die Entwicklung des Spiel wurde beauftragt von museum4punkt0 - Teilprojekt Zentrale wissenschaftliche Projektsteuerung. Die gestalterische sowie die inhaltliche Konzeption wurde in Zusammenarbeit mit Kindern von 6-12 Jahren erarbeitet. Mehr Informationen dazu  [im BLOG-Beitrag auf der Website von museum4punkt0.](https://www.museum4punkt0.de/ein-browsergame-entsteht-kinder-als-expertinnen-jury-einbinden/)</br></br>
![Aufstellbuch](https://github.com/museum4punkt0/images/blob/main/Z_Bewegungsbuch_Meggendorfer_Foto_SPK_Faulstich_ds100-1600x900.jpg)

# Kurzbeschreibung
"Rettet die Spielebücher - Robo gegen die Papierfresser" ist ein browserbasiertes Spiel, das mit Unity im Rahmen des Projekts museum4punkt0 erstellt wurde. </br></br>
[![Titelblatt](https://github.com/museum4punkt0/images/blob/main/Titelblatt_Papierfresser_Sprechblase_bunt.jpg)](https://www.museum4punkt0.de/browsergame/)

# Entstehungskontext
Dieses Spiel ist entstanden im Verbundprojekt museum4punkt0 – Digitale Strategien für das Museum der Zukunft.
Weitere Informationen: www.museum4punkt0.de

# Förderung
Das Projekt museum4punkt0 wird gefördert durch die Beauftragte der Bundesregierung für
Kultur und Medien aufgrund eines Beschlusses des Deutschen Bundestages.
![BKM-Logo](https://github.com/museum4punkt0/images/blob/2c46af6cb625a2560f39b01ecb8c4c360733811c/BKM_Fz_2017_Web_de.gif)

# Installation und Benutzung
Das Spiel "Rettet die Spielebücher - Robo gegen die Papierfresser" wurde mit unity 2019.1.0f2 erstellt.</br>
Das hier veröffentlichte Respository enthält den Build des unity-Projekts, der beim Export als unity WebGL-Target erzeugt wurde. Das Spiel ist somit in einem  Browser online lauffähig, sofern der Browser den unity WebGl Player untersützt. Zur Installation des Spiels müssen die in diesem Repositorium abgelegten Dateien nach dem Herunterladen entzippt,  auf einen Web-Server hochgeladen und in eine Website eingebettet werden. Durch Aufruf der index.html startet der unity WebGL-Player. Nach dem Laden der Assets steht das Spiel zur Benutzung bereit. Siehe [hier](https://www.museum4punkt0.de/browsergame/) das Beispiel auf der Website von museum4punkt0.     </br></br>
Eine Weiterentwicklung und Anpassung des Spiels muss auf dem Quellcode mit allen Unity-Komponenten aufsetzen. Die dafür benötigten Dateien können angefragt werden bei: [museum4punkt0](mailto:m4p0.z@smb.spk-berlin.de?subject=[GitHub]%20Rettet-die-Spielebuecher) /  [Andreas Richter](mailto:a.richter@smb.spk-berlin.de?subject=[GitHub]%20Rettet-die-Spielebuecher).  Um den Code innerhalb von Unity zu starten bedarf es weiterer Konfigurationen, die innerhalb von unity vorgenommen werden müssen. Es empfiehlt sich die Installation folgender Plugins aus dem unity AssetStore:

1. Airy UI: https://assetstore.unity.com/packages/tools/gui/airy-ui-easy-ui-animation-135898

2. Marvelous Techniques: https://assetstore.unity.com/packages/vfx/shaders/marvelous-techniques-37726

3. Tween: https://assetstore.unity.com/packages/tools/animation/tween-55983

# Credits
Auftraggeber/Rechteinhaber: museum4punkt0 </br>
Entwickler: Jens Blank, STUDIO JESTER BLANK


# Lizenz

Copyright © 2019, museum4punkt0 / Jens Blank, STUDIO JESTER BLANK

Dieser Code unterliegt der GNU General Public License v3.0. Näheres siehe [hier](https://github.com/museum4punkt0/Rettet-die-Spielebuecher/blob/main/LICENSE). 

Der Code enthält wiederverwendete Programmteile Dritter. Diese sind im Code markiert und auskommentiert. 
Um den Code zu starten, müssen die Kommentare an den entsprechenden Stellen entfernt werden.
Diese sind: 

1. Messenger.cs v1.0 by Magnus Wolffelt, magnus.wolffelt@gmail.com
Version 1.4 by Julie Iaccarino, biscuitWizard @ github.com
Inspired by and based on Rod Hyde's Messenger:
http://www.unifycommunity.com/wiki/index.php?title=CSharpMessenger

2. The A* Pathfinding Project 
The A* Pathfinding Project is an out-of-the-box pathfinding system
which along with ease of use has a large amount of features and blazing fast pathfinding.
The system has a Free version and a Pro version, both can found on my website (see below) and the Pro version can also be found in the Unity Asset Store
The A* Pathfinding Project was made by Aron Granberg
http://www.arongranberg.com
The license is the AssetStore Free License and the AssetStore Commercial License respectively for the Free and Pro versions of the project.

3. DUCK.Tween
see https://www.digitalruby.com/unity-plugins 
