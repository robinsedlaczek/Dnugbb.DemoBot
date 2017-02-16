using System;

namespace Dnugbb.DemoBot.Data
{
    public class EventProvider
    {
        public static Event[] Events
        {
            get
            {
                return new[]
                {
                    new Event()
                    {
                        Date = new DateTime(2017, 1, 12),
                        Address = "Friedrichstraße 187, 10117 Berlin",
                        Topic = "Conversational UIs & Intelligente Bots – Next big Thing oder nur Hype? Ein Überblick.",
                        Speaker = "Robin Sedlaczek",
                        SpeakerImage = "http://dnugbb.azurewebsites.net/wp-content/uploads/2016/12/Robin-klein-zoomed.jpg",
                        Abstract = "Anwendungen mit denen man zukünftig nur noch redet, und die durch maschinelles Lernen immer intelligenter werden. Chat-Plattformen, als zugrundliegende Infrastruktur, und die Cloud sind ihr Zuhause. Der Chat als Schnittstelle zwischen Mensch und Anwendung. Ein Trend, der jüngst immer mehr an Beachtung und Bedeutung gewinnt. Doch was genau passiert hier gerade eigentlich? Läuten CUIs und intelligente Bots die nächste Wende der digitalen Transformation ein, oder ist doch alles nur ein Hype? In dieser Session werden mögliche Anwendungsfälle betrachtet. CUIs, Bots und maschinelles Lernen werden thematisch eingeordnet, und ein Praxisbezug wird über das Microsoft Bot Framework und die Microsoft Cognitive Services hergestellt.",
                        EventUrl = new Uri("http://dnugbb.azurewebsites.net/events/conversational-uis-intelligente-bots-next-big-thing-oder-nur-hype-ein-ueberblick/")
                    },
                    new Event()
                    {
                        Date = new DateTime(2017, 2, 9),
                        Address = "Friedrichstraße 187, 10117 Berlin",
                        Topic = "Micro Apps mit UWP – Windows 10 Anniversary Update",
                        Speaker = "Hannes Preishuber",
                        SpeakerImage = "http://dnugbb.azurewebsites.net/wp-content/uploads/2017/01/HannesPreishuber.jpg",
                        Abstract = "Windows 10 wird von Microsoft nur mehr per Update gepflegt. In der zwischenzweit ist faktisch 10.2 existent. Offiziell als Windows 10 Anniversary Update bezeichnet und in Visual Studio mit der Build Nummer 14393. Da stecken eine Reihe interessanter Optionen drinnen, die recht deutlich machen warum eine native cross Platform Strategie immer Platz haben wird. In diesem Vortrag werden diese Geheimnisse gelüftet und gezeigt wie für die Windows Universal Plattform (UWP) ganz neue APP Architektur Konzepte realisiert werden können. Nichts was man morgen unbedingt benötigt, aber voll von Ideen.",
                        EventUrl = new Uri("http://dnugbb.azurewebsites.net/events/micro-apps-mit-uwp-windows-10-anniversary-update/")
                    },
                    new Event()
                    {
                        Date= new DateTime(2017, 3, 2),
                        Address = "Friedrichstraße 187, 10117 Berlin",
                        Topic = ".NET Core, MSSQL, Container und Kubernetes",
                        Speaker = "Matthias Fricke",
                        SpeakerImage = "http://dnugbb.azurewebsites.net/wp-content/uploads/2016/11/Thomas-Fricke.jpg",
                        Abstract = "Es sollen mit wenig Aufwand Microservices erstellt werden, die sich als Pods in Kubernetes orchestrieren, leicht ausrollen und updaten lassen. Mit Environment-Variablen und den eingebauten Secrets lassen sich komplexe Systeme einfach erstellen und aktualisieren. Beispiele für Java, Python und Ruby werden diskutiert. Mit der .NET Erfahrung und der Unterstützung der .NET User Gruppe könnten wir sogar einen Service bauen, der .NET in Kubernetes ausrollt.",
                        EventUrl = new Uri("http://dnugbb.azurewebsites.net/events/net-core-mssql-container-und-kubernetes/")
                    }
                };
            }
        }
    }
}