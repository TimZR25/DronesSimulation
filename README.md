# Drones Simulation

Всё было реализовано, кроме пары пунктов из "Опционально": выбор дрона для слежения и миникарта.

Дополнительно добавлено перемещение камеры и зум.

Главная логика:
=
Есть главный штаб, который раздаёт указания дронам в покое. Дрону назначается конечный пункт, в зависимости от состояния дрон соберёт или выгрузит ресурс. После сбора дрон возвращается в штаб. После выгрузки уходит в покой. Дрон строит маршрут к ближайшему незанятому ресурсу.
Ресурсы спавнятся на свободной поверхности, когда рядом нет препятствий и других ресурсов.

Использованные инструменты и подходы:
=
Ассет NavMeshPlus: https://github.com/h8man/NavMeshPlus (2D дополнение к NavMesh). 

Для дрона использована машина состояний (можно увидеть смену состояний по цветам).

Скриншоты:
=
![Gameplay Screenshot](https://github.com/TimZR25/DronesSimulation/blob/52cebf46bbbb769abfd9049caa7cb1b30495ef6e/Screenshoots/screenshot_1.png)
![Gameplay Screenshot](https://github.com/TimZR25/DronesSimulation/blob/52cebf46bbbb769abfd9049caa7cb1b30495ef6e/Screenshoots/screenshot_2.png)
![Gameplay Screenshot](https://github.com/TimZR25/DronesSimulation/blob/52cebf46bbbb769abfd9049caa7cb1b30495ef6e/Screenshoots/screenshot_3.png)
![Gameplay Screenshot](https://github.com/TimZR25/DronesSimulation/blob/52cebf46bbbb769abfd9049caa7cb1b30495ef6e/Screenshoots/screenshot_4.png)
