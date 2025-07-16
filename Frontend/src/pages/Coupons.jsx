import React, { useContext } from 'react';
import {
  Table,
  TableHeader,
  TableCell,
  TableFooter,
  TableContainer,
  Input,
  Button,
  Card,
  CardBody,
  Pagination,
} from '@windmill/react-ui';
import { FiPlus } from 'react-icons/fi';

import useAsync from '../hooks/useAsync.js';
import useFilter from '../hooks/useFilter.js';
import NotFound from '../components/table/NotFound.js';
import Loading from '../../../../../../../Users/husey/Downloads/Compressed/dashtar-admin-frontend-master/dashtar-admin-frontend-master/src/components/preloader/Loading.js';
import CouponServices from '../../../../../../../Users/husey/Downloads/Compressed/dashtar-admin-frontend-master/dashtar-admin-frontend-master/src/services/CouponServices.js';
import { SidebarContext } from '../../../../../../../Users/husey/Downloads/Compressed/dashtar-admin-frontend-master/dashtar-admin-frontend-master/src/context/SidebarContext.jsx';
import CouponTable from '../../../../../../../Users/husey/Downloads/Compressed/dashtar-admin-frontend-master/dashtar-admin-frontend-master/src/components/coupon/CouponTable.js';
import PageTitle from '../../../../../../../Users/husey/Downloads/Compressed/dashtar-admin-frontend-master/dashtar-admin-frontend-master/src/components/Typography/PageTitle.js';
import MainDrawer from '../../../../../../../Users/husey/Downloads/Compressed/dashtar-admin-frontend-master/dashtar-admin-frontend-master/src/components/drawer/MainDrawer.js';
import CouponDrawer from '../../../../../../../Users/husey/Downloads/Compressed/dashtar-admin-frontend-master/dashtar-admin-frontend-master/src/components/drawer/CouponDrawer.js';

const Coupons = () => {
  const { toggleDrawer } = useContext(SidebarContext);
  const { data, loading } = useAsync(CouponServices.getAllCoupons);

  const {
    handleSubmitCoupon,
    couponRef,
    dataTable,
    serviceData,
    totalResults,
    resultsPerPage,
    handleChangePage,
  } = useFilter(data);

  return (
    <>
      <PageTitle>Coupons</PageTitle>

      <MainDrawer>
        <CouponDrawer />
      </MainDrawer>

      <Card className="min-w-0 shadow-xs overflow-hidden bg-white dark:bg-gray-800 mb-5">
        <CardBody>
          <form
            onSubmit={handleSubmitCoupon}
            className="py-3 grid gap-4 lg:gap-6 xl:gap-6 md:flex xl:flex"
          >
            <div className="flex-grow-0 md:flex-grow lg:flex-grow xl:flex-grow">
              <Input
                ref={couponRef}
                type="search"
                className="border h-12 text-sm focus:outline-none block w-full bg-gray-100 border-transparent focus:bg-white"
                placeholder="Search by coupon code/name"
              />
            </div>
            <div className="w-full md:w-56 lg:w-56 xl:w-56">
              <Button onClick={toggleDrawer} className="w-full rounded-md h-12">
                <span className="mr-3">
                  <FiPlus />
                </span>
                Add Coupon
              </Button>
            </div>
          </form>
        </CardBody>
      </Card>

      {loading ? (
        <Loading loading={loading} />
      ) : serviceData.length !== 0 ? (
        <TableContainer className="mb-8">
          <Table>
            <TableHeader>
              <tr>
                <TableCell>ID</TableCell>
                <TableCell>Start Date</TableCell>
                <TableCell>End Date</TableCell>
                <TableCell>Campaigns Name</TableCell>
                <TableCell>Code</TableCell>
                <TableCell>Percentage</TableCell>
                <TableCell>Product Type</TableCell>
                <TableCell>Status</TableCell>
                <TableCell className="text-right">Actions</TableCell>
              </tr>
            </TableHeader>
            <CouponTable coupons={dataTable} />
          </Table>
          <TableFooter>
            <Pagination
              totalResults={totalResults}
              resultsPerPage={resultsPerPage}
              onChange={handleChangePage}
              label="Table navigation"
            />
          </TableFooter>
        </TableContainer>
      ) : (
        <NotFound title="Coupon" />
      )}
    </>
  );
};

export default Coupons;
