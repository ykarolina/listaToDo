Karolina Cieniecka klasa 2P

Projekt listyToDo

Specyfikacja wymagań 

    Tytuł i krótki opis 

Lista ToDo to aplikacja desktopowa dla uczniów szkół średnich, która pomaga w organizowaniu codziennych zadań. Dzięki niej możesz zaplanować naukę, śledzić postępy i nigdy nie zapomnieć o zbliżających się terminach.

Aplikacja umożliwia tworzenie, przeglądanie, edytowanie i usuwanie zadań związanych z nauką. Zadania są posortowane według daty i priorytetu, a system powiadomień przypomina o zbliżających się terminach.


    Służy do:

Organizacji nauki i zadań domowych

Przypominania o terminach

Śledzenia postępów (status ukończenia)

Lepszego zarządzania czasem



    Przykłady użycia:

Planowanie nauki przed ważnym terminem – dodaj zadanie z wysokim priorytetem i określonym deadlinem.

Zarządzanie codziennymi obowiązkami szkolnymi – twórz listę rzeczy do zrobienia na każdy dzień.

Śledzenie postępów w nauce – oznaczaj wykonane zadania, aby mieć kontrolę nad tym, co już zrobione.

Otrzymywanie przypomnień o zbliżających się terminach – aplikacja powiadomi Cię dzień wcześniej.

Porządkowanie zadań – usuwaj wykonane zadania lub czyść całą listę na zakończenie tygodnia.


![image](https://github.com/user-attachments/assets/17cf8a15-c7df-4270-9369-c98b802bfa1d)



    WYMAGANIA FUNKCJONALNE

-DODAWANIE ZADANIA

Uczeń może dodać nowe zadanie, podając:

• tytuł (pole tekstowe z podpowiedzią „Wpisz tytuł...”),

• datę wykonania (kontrolka DatePicker),
  
• krótki opis (pole tekstowe z podpowiedzią „Dodaj krótki opis”),
  
• priorytet (lista: niski, średni, wysoki).
  
Żadne pole nie może pozostać puste.

Zadanie dodaje się po kliknięciu przycisku dodaj zadanie.

-WYŚWIETLANIE LISTY ZADAŃ
  
Główne okno aplikacji prezentuje wszystkie zadania w formie listy.

Lista pokazuje podstawowe informacje o zadaniu (tytuł, data, statusem(ukończone/nieukończone) ).

-OZNACZENIE ZADAŃ JAKO UKOŃCZONE

Użytkownik może ręcznie oznaczyć zadanie jako wykonane, klikając przycisk „zakończ” przy zadaniu. Status zadania jest wtedy wizualnie zmieniony (np. zmiana tekstu przycisku na „zakończone”). 

-USUWANIE ZADAŃ

Użytkownik może:

-usunąć pojedyncze zadanie (przycisk z ikoną kosza przy zadaniu) ,

-wyczyścić całą listę zadań (przycisk „usuń wszystkie”).

-SORTOWANIE ZADAŃ

Lista zadań jest automatycznie sortowana według:

daty wykonania (rosnąco),

priorytetu (malejąco – najpierw najważniejsze).

statusu ukończenia (ukończone, nieukończone).

-FILTROWANIE ZADAŃ
Użytkownik może filtrować listę według:

wszystkie,ukończone,do zrobienia.

-POWIADOMIENIA

Aplikacja wyświetla alert lub powiadomienie systemowe na 1 dzień przed terminem zadania.

Aplikacja wyświetla alert jeśli którekolwiek z pól do wypełnienia będzie puste gdy zostanie kliknięty przycisk dodaj zadanie.


-AUTOMATYCZNY ZAPIS

Dane są zapisywane automatycznie po każdej zmianie (dodaniu, usunięciu, oznaczeniu jako ukończone).

Po ponownym uruchomieniu aplikacji lista zadań zostaje przywrócona.



    WYMAGANIA NIEFUNKCJONALNE
• Interfejs ma być intuicyjny i prosty w obsłudze dla uczniów w wieku szkolnym. 

• Aplikacja działa na systemie Windows, zbudowana w technologii WPF. 

• Aplikacja jest przygotowana do rozbudowy (np. dodanie kategorii zadań, powiadomień). 

• Aplikacja posiada estetyczny, spójny design z zaokrąglonymi elementami i pastelowymi kolorami. 

• Aplikacja posiada estetyczny, spójny design z zaokrąglonymi elementami z jasnym układem i czytelną czcionką (Comic Sans MS). 



    Instalacja:
1,Pobierz repozytorium:

git clone https://github.com/ykarolina/listaToDo.git


2.Otwórz projekt w Visual Studio 2022+

3.Zbuduj i uruchom projekt:

4.Wybierz konfigurację Debug

5.Kliknij Start

    Technologie

WPF (Windows Presentation Foundation) – interfejs graficzny

C# – język programowania

.NET – środowisko uruchomieniowe

Plik lokalny (np. XML/JSON) – do przechowywania danych


    Licencja

Projekt jest udostępniany na licencji MIT.

