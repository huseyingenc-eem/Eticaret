import { Timeline, type TimelineItem } from "@/components/ui";
import { randomId } from "@/utils/randomId";

const items: TimelineItem[] = [
    {
        id: randomId(),
        title: "User Photo Changed",
        description: "John Doe changed his avatar photo",
        timestamp: new Date(Date.now() - 12 * 60 * 1000).toLocaleString("tr-TR"),
        status: "info",
    },
    {
        id: randomId(),
        title: "Video Added",
        description: "Mores Clarke added new video",
        timestamp: new Date(Date.now() - 2 * 60 * 60 * 1000).toLocaleString("tr-TR"),
        status: "current",
    },
    {
        id: randomId(),
        title: "Design Completed",
        description: "Robert Nolan completed the design of the CRM application",
        timestamp: new Date(Date.now() - 3 * 60 * 60 * 1000).toLocaleString("tr-TR"),
        status: "complete",
    },
    {
        id: randomId(),
        title: "ER Diagram",
        description: "Team completed the ER diagram app",
        timestamp: new Date(Date.now() - 24 * 60 * 60 * 1000).toLocaleString("tr-TR"),
        status: "upcoming",
    },
    {
        id: randomId(),
        title: "Weekly Report",
        description: "The weekly report was uploaded",
        timestamp: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000).toLocaleString("tr-TR"),
        status: "error",
    },
];

export default function TimelinePage() {
    return (
        <div className="mx-auto max-w-xl">
            <Timeline items={items} orientation="horizontal" align="left"  />
        </div>
    );
}