import React from 'react';
import {TableBody, TableRow, TableCell, Avatar} from '@windmill/react-ui';

import MainModal from '../modal/MainModal.jsx';
import MainDrawer from '../drawer/MainDrawer.jsx';
import ShowHideButton from '../table/ShowHideButton.jsx';
import CategoryDrawer from '../drawer/CategoryDrawer.jsx';
import useToggleDrawer from '../../hooks/useToggleDrawer';
import EditDeleteButton from '../table/EditDeleteButton.jsx';

const CategoryTable = ({categories}) => {
    const {serviceId, handleModalOpen, handleUpdate} = useToggleDrawer();

    return (
        <>
            <MainModal id={serviceId}/>
            <MainDrawer>
                <CategoryDrawer id={serviceId}/>
            </MainDrawer>

            <TableBody>
                {categories?.map((parent) => (
                    <TableRow key={parent.id}>
                        <TableCell>{parent.id ?? 'N/A'}</TableCell>
                        <TableCell className="text-sm font-semibold">{parent.name}</TableCell>
                        <TableCell className="font-medium text-sm">
                            <div className="flex flex-row flex-wrap gap-1">
                                {parent?.children?.map((child) => (
                                    <span
                                        key={child.id}
                                        className="bg-gray-200 text-gray-500 rounded-full inline-flex items-center justify-center px-2 py-1 text-xs font-semibold mt-2 dark:bg-gray-700 dark:text-gray-300"
                                    >
                                        {child.name}
                                    </span>
                                ))}
                            </div>
                        </TableCell>
                        <TableCell>{parent.children?.length ?? 0}</TableCell>
                        <TableCell className="text-sm">
                            {parent.parentId === null ? 'Ana Kategori' : 'Alt Kategori'}
                        </TableCell>
                        <TableCell>
                            <ShowHideButton id={parent.id} status={parent.isActive}/>
                        </TableCell>
                        <TableCell>
                            <EditDeleteButton
                                id={parent.id}
                                handleUpdate={handleUpdate}
                                handleModalOpen={handleModalOpen}
                            />
                        </TableCell>
                    </TableRow>
                ))}
            </TableBody>
        </>
    );
};

export default CategoryTable;
