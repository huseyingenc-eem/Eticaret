import React from 'react';
import { Dropdown } from '@/components/ui';
import { BellRing, Clock, MoveRight, ShoppingBag } from 'lucide-react';
import SimpleBar from 'simplebar-react';

import avatar3 from '@/assets/images/users/avatar-3.png';
import avatar5 from '@/assets/images/users/avatar-5.png';

type Notification = { id: number; type: 'follower'|'mention'|'invite'; image?: string; boldName?: string; name?: string; price?: string; time: string; date: string; description?: string };

const list: Notification[] = [
    { id: 1, type: 'follower', image: avatar3, boldName: '@willie_passem', name: 'followed you', time: '4 sec', date: 'Wed 03:42 PM' },
    { id: 2, type: 'mention', image: avatar5, boldName: '@caroline_jessica', name: 'commented on your post', time: '15 min', description: 'Amazing! Fast, to the point...', date: 'Wed 03:42 PM' },
    { id: 3, type: 'invite', name: 'Purchased a business plan for', price: '$199.99', time: 'Yesterday', date: 'Mon 11:26 AM' },
    { id: 4, type: 'mention', boldName: '@scott', name: 'liked your post', time: '1 Week', date: 'Thu 06:59 AM' },
];

const NotificationDropdown: React.FC = () => {
    const [filter, setFilter] = React.useState<'all' | Notification['type']>('all');

    const filtered = list.filter(i => filter === 'all' || i.type === filter);

    return (
        <Dropdown className="relative flex items-center h-header">
            <Dropdown.Trigger
                as="button"
                id="notificationDropdown"
                className="inline-flex relative justify-center items-center p-0 size-[37.5px] rounded-md bg-topbar text-topbar-item hover:bg-topbar-item-bg-hover"
            >
                <BellRing className="inline-block size-5" />
                <span className="absolute top-0 right-0 flex w-1.5 h-1.5">
          <span className="absolute inline-flex w-full h-full rounded-full opacity-75 animate-ping bg-sky-400"></span>
          <span className="relative inline-flex w-1.5 h-1.5 rounded-full bg-sky-500"></span>
        </span>
            </Dropdown.Trigger>

            <Dropdown.Content
                placement="right-end"
                className="!top-4 min-w-[22rem] lg:min-w-[26rem] p-0 bg-white dark:bg-zinc-700"
            >
                <div className="p-4">
                    <h6 className="mb-4 text-16">Notifications <span className="inline-flex items-center justify-center size-5 ml-1 text-[11px] font-medium border rounded-full text-white bg-orange-500 border-orange-500">{list.length}</span></h6>

                    <div className="flex gap-2 text-xs">
                        {(['all','mention','follower','invite'] as const).map(k => (
                            <button key={k} onClick={() => setFilter(k)} className={`px-2 py-1 rounded ${filter===k?'bg-slate-200 text-custom-600':'bg-slate-100'}`}>
                                {k}
                            </button>
                        ))}
                    </div>
                </div>

                <SimpleBar className="max-h-[350px]">
                    <div className="flex flex-col">
                        {filtered.map((item) => (
                            <div key={item.id} className="flex gap-3 p-4 hover:bg-slate-50">
                                {item.image ? (
                                    <img src={item.image} className="size-10 rounded-md object-cover" />
                                ) : (
                                    <div className="flex items-center justify-center size-10 bg-red-100 rounded-md shrink-0">
                                        <ShoppingBag className="size-5" />
                                    </div>
                                )}

                                <div className="grow">
                                    <h6 className="mb-1 font-medium">
                                        {item.boldName && <b>{item.boldName} </b>}
                                        {item.name} {item.price && <span className="text-red-500">{item.price}</span>}
                                    </h6>
                                    <p className="text-sm text-slate-500 mb-0">
                                        <Clock className="inline-block size-3 mr-1" /> {item.date}
                                    </p>
                                    {item.description && <div className="mt-2 p-2 rounded bg-slate-100 text-slate-600">{item.description}</div>}
                                </div>

                                <div className="self-start text-xs text-slate-500">{item.time}</div>
                            </div>
                        ))}
                    </div>
                </SimpleBar>

                <div className="flex items-center gap-2 p-4 border-t border-slate-200">
                    <div className="grow">
                        <a href="#!">Manage Notification</a>
                    </div>
                    <div className="shrink-0">
                        <button className="px-2 py-1.5 text-xs text-white bg-custom-500 rounded">View All Notification <MoveRight className="inline-block size-3 ml-1" /></button>
                    </div>
                </div>
            </Dropdown.Content>
        </Dropdown>
    );
};

export default NotificationDropdown;
