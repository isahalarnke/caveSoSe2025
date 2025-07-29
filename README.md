# Corpus in Speculo / CAVE-Projekt SoSe 2025

**Corpus in Speculo** ist ein künstlerisches Projekt, das innerhalb einer immersiven Umgebung neue Perspektiven auf Körper, Identität und Bewegung eröffnet. Der eigene Körper wird durch einen digitalen Avatar gespiegelt. Bewegungen erscheinen sowohl in Echtzeit als auch als „Echo“ vergangener Bewegungsabläufe. Dadurch entsteht ein digitales Selbst, das sich vom physischen Körper distanziert und neue Perspektiven auf Identität, Ausdruck und körperliche Wahrnehmung ermöglicht.
 
[Demovideo von Corpus in Speculo](https://mediathek.htw-berlin.de/category/video/cave-projekt-sommersemester-2025-corpus-in-speculo/9fb6709c890c5910c8080be708665452/44)
## Konzept

Das Projekt verfolgt das Ziel, normative Vorstellungen von Körper, Bewegung und Geschlecht zu hinterfragen. Der Avatar ist bewusst **fragmentiert** und **glitchhaft** als künstlerische Strategie, um alternative Formen von Körperlichkeit sichtbar zu machen.

Durch den Einsatz der **CAVE-Technologie** (Cave Automatic Virtual Environment) anstelle von VR-Brillen entsteht ein kollektives, physisch begehbares Erlebnis. Im Gegensatz zu herkömmlichen VR-Setups fördert die CAVE ein soziales, interaktives Umfeld, das Raum für kreatives Miteinander bietet.

Tänzer:innen, Performer:innen und alle bewegungsaffinen Menschen sind eingeladen, ihre gewohnten Bewegungsmuster zu hinterfragen und neue Ausdrucksformen zu erforschen.

## Zielsetzung der Features

- Spiegelung des Körpers durch einen digitalen Avatar
- Bewegungs-Echo der vergangenen Bewegungen auf weitere Avatare
- Visualisierungen mehrerer getrackter Personen (mind. 2)
- Kollisionseffekte beim Klatschen der Hände

## Technische Umsetzung

- **Unity Version:** [2022.3.59f1]
- **Kinect for Windows SDK** Kinect V2
- **CAVE Package** – für multiwandige Projektionsumgebung und Kinect Tracking
  [→ Zum CAVE-Package der HTW Berlin](https://github.com/FKI-HTW/CAVE#upm)

Die Kinect erfasst die Positionen der Gelenke der interagierenden Person. Diese Daten werden auf einen virtuellen Avatar verarbeitet, sodass eine Verbindung zwischen physischem Körper und seinem digitalen Echo entsteht. Die Projektion erfolgt auf 3 Raum- und eine Bodenwand.



## Projektstruktur und Skripte
### 1. `PositionTransferMultiple.cs`

> **Funktion:** Überträgt und spiegelt die Bewegungsdaten von der Kinect vom (CAVE--> Kinect Tracker --> Kinect Actor #) auf einen Avatar. Muss einem leeren GameObject in der Szene zugeteilt werden.

- Liest ausgewählte Gelenkpositionen von "Kinect Actor #ID" GameObject
- Erstellt und verwaltet 3D-Körperteile (Head, Hand, Body) für jeden Avatar und fügt das BodyCollision.cs an sie an
- Visualisiert Verbindungen der Gelenke per `LineRenderer`
- Spiegelt die Bewegungen an einer wählbaren virtuellen Ebene
- Körperteil Prefabs, Sound und Partikelsystem für die Kollisionen können über den Inspektor gesetzt werden
---

### 2. `MovementEchoMultiple.cs`

> **Funktion:** Nimmt Bewegungen (Körperteilpositionen aus PositionTransferMultiple.cs) über selbstfestgelegten einen Zeitraum auf und spielt sie zeitversetzt als "Echos" erneut ab. Muss einem leeren GameObject in der Szene zugeteilt werden.

- Aufzeichnung der Gelenkpositionen pro Avatar
- Periodische Replays mit versetzter zufälliger räumlicher Position (Vorgegene Spawn Area)
- Verbindet gespawnte Echo-Körperteile mit Linien
- Fügt jedem Echo-Körperteil `BodyCollision.cs` hinzu

- Recording Sound, FPS (für die Schnelligkeit der Bewegung: per default 200) und Spawn Area (Box Collider ohne Mesh) sowie Kollisionssound und Partikelsystem können über den Inspektor gesetzt werden
---

### 3. `BodyCollision.cs`

> **Funktion:** Erkennt Kollisionen zwischen bestimmten Körperteilen (z. B. Hände) und erzeugt visuelle & auditive Effekte.

- Prüft auf Kollisionen
- Bei Hand-zu-Hand-Kollisionen:
  - spawnt gesetztes Partikelsystem an der Stelle
  - erzeugt einen Soundeffekt (gesetzte Audio Source)
---

## Verwendete Assets
- Materials: [Yughues Free Ground Materials](https://assetstore.unity.com/packages/2d/textures-materials/nature/yughues-free-ground-materials-13001?srsltid=AfmBOoryd4hSxEnBYxEOCIZqBuWTdLrISyRfA1lpi2GBBwu-4a-2-aue), [Deep Space Skybox Pack](https://assetstore.unity.com/packages/2d/textures-materials/deep-space-skybox-pack-11056), sowie bereits im Cave Package vorhandene Assets
- Sounds: [Free Sound](https://freesound.org)


## Team

Das Projekt entstand im Rahmen des Projektstudiums SoSe 2025 im Studiengang Informatik in Kultur und Gesundheit an der HTW Berlin.

- Studierende: Isabell Arnke, Pascalle Marie Strübel, Mostafa Kassem
- Betreuung: Prof. Dr.-Ing. Thomas Jung

