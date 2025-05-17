import React, { useContext } from 'react';
import { Button } from '@windmill/react-ui';

import Error from '../components/form/Error.jsx';
import useStaffSubmit from '../hooks/useStaffSubmit.js';
import LabelArea from '../components/form/LabelArea.jsx';
import InputArea from '../components/form/InputArea.jsx';
import { AdminContext } from '../contexts/AdminContext.jsx';
import SelectRole from '../components/form/SelectRole.jsx';
import PageTitle from '../components/Typography/PageTitle.jsx';
import Uploader from '../components/image-uploader/Uploader.jsx';

const EditProfile = () => {
  const {
    state: { adminInfo },
  } = useContext(AdminContext);

  const { register, handleSubmit, onSubmit, errors, imageUrl, setImageUrl } =
    useStaffSubmit(adminInfo._id);

  return (
    <>
      <PageTitle>Edit Profile</PageTitle>
      <div className="container p-6 mx-auto bg-white  dark:bg-gray-800 dark:text-gray-200 rounded-lg">
        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="p-6 flex-grow scrollbar-hide w-full max-h-full">
            <div className="grid grid-cols-6 gap-3 md:gap-5 xl:gap-6 lg:gap-6 mb-6">
              <LabelArea label="Profile Picture" />
              <div className="col-span-8 sm:col-span-4">
                <Uploader imageUrl={imageUrl} onChange={setImageUrl} />
              </div>
            </div>

            <div className="grid grid-cols-6 gap-3 md:gap-5 xl:gap-6 lg:gap-6 mb-6">
              <LabelArea label="Name" />
              <div className="col-span-8 sm:col-span-4">
                <InputArea
                  register={register}
                  label="Name"
                  name="name"
                  type="text"
                  placeholder="Your Name"
                />
                <Error errorName={errors.name} />
              </div>
            </div>

            <div className="grid grid-cols-6 gap-3 md:gap-5 xl:gap-6 lg:gap-6 mb-6">
              <LabelArea label="Email" />
              <div className="col-span-8 sm:col-span-4">
                <InputArea
                  register={register}
                  label="Email"
                  name="email"
                  type="text"
                  placeholder="Email"
                />
                <Error errorName={errors.email} />
              </div>
            </div>

            <div className="grid grid-cols-6 gap-3 md:gap-5 xl:gap-6 lg:gap-6 mb-6">
              <LabelArea label="Contact Number" />
              <div className="col-span-8 sm:col-span-4">
                <InputArea
                  register={register}
                  label="Contact Number"
                  name="phone"
                  type="text"
                  placeholder="Contact Number"
                />
                <Error errorName={errors.phone} />
              </div>
            </div>

            <div className="grid grid-cols-6 gap-3 md:gap-5 xl:gap-6 lg:gap-6 mb-6">
              <LabelArea label="Your Role" />
              <div className="col-span-8 sm:col-span-4">
                <SelectRole register={register} label="Role" name="role" />
                <Error errorName={errors.role} />
              </div>
            </div>
          </div>

          <div className="flex flex-row-reverse pr-6 pb-6">
            <Button type="submit" className="h-12 px-6">
              {' '}
              Update Profile
            </Button>
          </div>
        </form>
      </div>
    </>
  );
};

export default EditProfile;
