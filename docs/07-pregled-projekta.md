# Pregled prevzetega projekta

Pregled je narejen na osnovi dokumentacije v `docs/`, README.md ter kode v `backend/SolarGuard.Api` in `frontend/src`.

## Implementirane funkcionalnosti MVP

MVP po [docs/02-funkcionalnosti-in-obseg.md](02-funkcionalnosti-in-obseg.md) obsega funkcionalnosti 2–7. Vse so implementirane na backendu in frontendu:

| # | Funkcionalnost | Status | Kje v kodi |
|---|---|---|---|
| 2 | Pregled proizvodnje v realnem času (graf dan/teden/mesec/leto) | implementirano | `MeasurementsController`, `ProductionService`/`ProductionRules`, `components/chart`, `dashboard-view` |
| 3 | Primerjava dejanske proti pričakovani proizvodnji | implementirano | `ProductionService` (model pričakovane proizvodnje iz lokacije/vremena), `OpenMeteoClient`, graf na dashboardu |
| 4 | Samodejno zaznavanje odstopanj (anomalij) | implementirano | `AlarmDetector`, `AlarmRules`, `AlarmDetectionService` (ozadnji servis, urno preverjanje), `AlarmsController` |
| 5 | Samodejno ustvarjanje servisnega naloga in dodelitev serviserju | implementirano | `AlarmDetector` ob kritičnem alarmu ustvari `ServiceOrder` in ga dodeli `installation.TechnicianId` oz. administratorju, če serviser ni določen |
| 6 | Sledenje statusu servisnega naloga | implementirano | `ServiceOrdersController.ChangeStatus` (vrstni red statusov, obvezen `Summary` ob zaključku, zapis zgodovine v `ServiceOrderChanges`), `service-order-list` |
| 7 | Izračun finančnega prihranka | implementirano | `SavingsService`/`SavingsRules`, `InstallationsController`, `savings-card` |

Projekt gre dejansko precej dlje od MVP-ja: po README.md in pregledu kode so dodatno implementirane tudi funkcionalnosti 1 (registracija/prijava, JWT), 8 (upravljanje naprav), 9 (napoved proizvodnje, `ForecastService`/`ForecastRules`), 10 (obveščanje po e-pošti in v aplikaciji, `NotificationMailer`, `MilestoneCheckService`), 11 (servisna zgodovina naprave), 12 (spremljanje garancij, `WarrantyChecker`/`WarrantyRules`) in 15 (administratorski pregled, `AdminController`).

Neimplementirani ostajata:
- **Funkcionalnost 13** – izvoz poročil (PDF/CSV): v kodi ni mogoče najti nobenega izvoznega mehanizma (ni PDF/CSV generatorja niti ustrezne poti v API-ju ali frontendu).
- **Funkcionalnost 14** – primerjava z drugimi sistemi (benchmarking): v modelu obstaja `ProductionComparison`, ki pa se po pregledu kode uporablja za primerjavo dejanske/pričakovane proizvodnje (funkcionalnost 3), ne za primerjavo med sistemi drugih uporabnikov. Benchmarking funkcionalnost ni implementirana.

README.md to stanje potrjuje neposredno: "US-01 do US-12 in US-15 so implementirane ... US-13 ... in US-14 ... sta neobvezni in še nista narejeni."

## Ustreznost implementacije uporabniškim zgodbam

Preverjenih je 5 uporabniških zgodb iz [docs/05-uporabniske-zgodbe.md](05-uporabniske-zgodbe.md), ki pokrivajo jedro MVP (verigo proizvodnja → zaznava odstopanja → servisni nalog).

### US-02 – Pregled proizvodnje v realnem času
Zadoščeno. Graf prikazuje proizvodnjo z možnostjo preklopa med dan/teden/mesec/leto (`chart` komponenta, `dashboard-view`), podatki prihajajo iz `MeasurementsController`.

### US-03 – Primerjava dejanske proti pričakovani proizvodnji
Zadoščeno. `ProductionService` izračuna pričakovano proizvodnjo na podlagi lokacije in vremenske napovedi (`OpenMeteoClient`), krivulja je prikazana poleg dejanske na dashboardu. Obnašanje ob manjkajoči lokaciji ni bilo posebej preverjeno v kodi.

### US-04 – Samodejno zaznavanje odstopanj (anomalij)
Zadoščeno, z manjšo razliko glede na kriterije. `AlarmRules.LowProduction` uporablja pragove 20 % (opozorilo) in 50 % (kritično) čez 3 zaporedne ure, kar ustreza zgodbi. Manjkajoče meritve nad 2 uri sprožijo ločen alarm tipa `MissingMeasurements`. Zaznavanje teče urno prek `AlarmDetectionService`, aktivni alarmi se sinhronizirajo (`Sync`), tako da se podvojeni alarmi v istem oknu ne ustvarjajo znova – kriterij o neustvarjanju duplikatov je izpolnjen.

### US-05 – Samodejno ustvarjanje servisnega naloga in dodelitev serviserju
Zadoščeno. Ob zaznavi kritičnega alarma `AlarmDetector` ustvari servisni nalog in ga dodeli serviserju sistema (`installation.TechnicianId`); če ta ni določen, se nalog dodeli administratorju (`FindAdminId`). Obvestilo serviserju v 1 minuti ni bilo posebej preverjeno (odvisno od `NotificationMailer`/urnika), velja preveriti ob zagonu.

### US-06 – Sledenje statusu servisnega naloga
Zadoščeno. `ServiceOrdersController.ChangeStatus` dovoljuje samo prehod na naslednji status v vrstnem redu (`(int)request.Status != (int)order.Status + 1` vrne `Conflict`), prehod v "Zaključen" zahteva neprazen `Summary`, vsaka sprememba se zapiše v `ServiceOrderChanges` s časom in uporabnikom. S tem so vsi glavni acceptance criteria izpolnjeni.

## Zaključek

Vseh 6 MVP funkcionalnosti (2–7) je implementiranih in skladnih z ustreznimi uporabniškimi zgodbami. Projekt presega obseg MVP-ja (implementirane so tudi funkcionalnosti 1, 8–12, 15), medtem ko neobvezni funkcionalnosti 13 (izvoz poročil) in 14 (benchmarking) manjkata, kar je skladno z navedbo v README.md.
