// SimpleTable.tsx
import type { FC } from "react";

type Row = {
    id: number;
    name: string;
    email: string;
    role: string;
    status: "Active" | "Invited" | "Suspended";
};

const sampleRows: Row[] = [
    { id: 1, name: "Aylin Demir", email: "aylin@example.com", role: "Admin", status: "Active" },
    { id: 2, name: "Berk Yılmaz", email: "berk@example.com", role: "Editor", status: "Invited" },
    { id: 3, name: "Ceren Kaya", email: "ceren@example.com", role: "Viewer", status: "Suspended" },
];

const pillClass = (s: Row["status"]) => {
    switch (s) {
        case "Active":
            return "ring-green-600/20 text-green-700 dark:text-green-300";
        case "Invited":
            return "ring-blue-600/20 text-blue-700 dark:text-blue-300";
        case "Suspended":
            return "ring-rose-600/20 text-rose-700 dark:text-rose-300";
    }
};

const SimpleTable: FC<{ rows?: Row[] }> = ({ rows = sampleRows }) => {
    return (
        <div className="overflow-x-auto">
            <table className="min-w-[640px] w-full border-collapse text-sm">
                <thead className="bg-gray-50 dark:bg-gray-900/40">
                <tr className="text-left">
                    <th className="px-4 py-3 font-semibold text-gray-700 dark:text-gray-200">ID</th>
                    <th className="px-4 py-3 font-semibold text-gray-700 dark:text-gray-200">Name</th>
                    <th className="px-4 py-3 font-semibold text-gray-700 dark:text-gray-200">Email</th>
                    <th className="px-4 py-3 font-semibold text-gray-700 dark:text-gray-200">Role</th>
                    <th className="px-4 py-3 font-semibold text-gray-700 dark:text-gray-200">Status</th>
                </tr>
                </thead>

                <tbody className="divide-y divide-gray-200 dark:divide-gray-800">
                {rows.map((r) => (
                    <tr key={r.id} className="hover:bg-gray-50/60 dark:hover:bg-gray-900/30">
                        <td className="px-4 py-3 text-gray-900 dark:text-gray-100">{r.id}</td>
                        <td className="px-4 py-3 text-gray-900 dark:text-gray-100">{r.name}</td>
                        <td className="px-4 py-3 text-gray-600 dark:text-gray-300">{r.email}</td>
                        <td className="px-4 py-3 text-gray-700 dark:text-gray-200">{r.role}</td>
                        <td className="px-4 py-3">
                <span className={`inline-flex items-center rounded-full px-2 py-1 text-xs font-medium ring-1 ring-inset ${pillClass(r.status)}`}>
                  {r.status}
                </span>
                        </td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
};

export default SimpleTable;
