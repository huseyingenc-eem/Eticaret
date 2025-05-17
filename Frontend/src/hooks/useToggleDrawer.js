import { useContext, useEffect, useState } from 'react';
import { SidebarContext } from '../contexts/SidebarContext.jsx';

const useToggleDrawer = () => {
  const [serviceId, setServiceId] = useState('');
  const { toggleDrawer, isDrawerOpen, toggleModal } =
    useContext(SidebarContext);

  const handleUpdate = (id) => {
    setServiceId(id);
    toggleDrawer();
  };

  const handleModalOpen = (id) => {
    setServiceId(id);
    toggleModal();
  };

  useEffect(() => {
    if (!isDrawerOpen) {
      setServiceId();
    }
  }, [isDrawerOpen]);

  return {
    serviceId,
    handleModalOpen,
    handleUpdate,
  };
};

export default useToggleDrawer;
