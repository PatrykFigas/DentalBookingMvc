APLIKACJA DENTAL BOOKING

Aplikacja webowa stworzona w technologii ASP.NET Core MVC, umożliwiająca rezerwację wizyt stomatologicznych online. System obsługuje role użytkownika oraz administratora i pozwala na pełne zarządzanie terminami oraz rezerwacjami.
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Cel projektu

Celem projektu było stworzenie aplikacji rezerwacyjnej opartej o wzorzec architektoniczny MVC (Model–View–Controller), spełniającej wymagania aplikacji typu CRUD z relacyjną bazą danych.

System umożliwia:

- przeglądanie dostępnych usług stomatologicznych,

- rezerwację wizyt online,

- zarządzanie rezerwacjami przez administratora,

- kontrolę dostępności terminów,

- bezpieczną autoryzację użytkowników z wykorzystaniem ASP.NET Core Identity.

--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Funkcjonalności użytkownika

Użytkownik końcowy może:

- zarejestrować się i zalogować do systemu,

- przeglądać listę usług stomatologicznych,

- wybrać dentystę i dostępny termin,

- zarezerwować wizytę,

- przeglądać swoje wizyty w zakładce „Moje wizyty”,

- edytować lub anulować rezerwację (min. 24h przed wizytą).


Funkcjonalności administratora

Panel administratora umożliwia:

- dodawanie i zarządzanie terminami wizyt,

- przegląd wszystkich rezerwacji użytkowników,

- zatwierdzanie lub odrzucanie rezerwacji,

- kontrolę dostępności terminów (wolny / zajęty).

Dostęp do panelu mają wyłącznie użytkownicy z rolą Admin.

--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Struktura bazy danych

Aplikacja korzysta z relacyjnej bazy danych SQL Server. Główne tabele:

- AspNetUsers – użytkownicy systemu (Identity),

- Services – usługi stomatologiczne,

- Dentists – dane dentystów,

- TimeSlots – dostępne terminy wizyt,

- Reservations – rezerwacje użytkowników.


Relacje umożliwiają:

- przypisanie rezerwacji do użytkownika,

- powiązanie wizyty z usługą,

- rezerwację konkretnego terminu u wybranego dentysty,

- kontrolę dostępności poprzez pola IsActive i IsBooked.

--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Zastosowane technologie

ASP.NET Core MVC – architektura aplikacji

Entity Framework Core – ORM i migracje bazy danych

SQL Server – relacyjna baza danych

ASP.NET Core Identity – autoryzacja i role

Razor Views – generowanie widoków

Bootstrap – stylowanie interfejsu

JavaScript – walidacja i interakcje

--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Podsumowanie

Projekt realizuje pełny proces rezerwacji wizyt online z podziałem na role oraz bezpieczną autoryzacją użytkowników.
Zastosowanie Entity Framework Core oraz ASP.NET Core Identity pozwoliło na stworzenie skalowalnego i bezpiecznego systemu zgodnego z dobrymi praktykami tworzenia aplikacji webowych.
