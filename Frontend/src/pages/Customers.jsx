import React from 'react';
import {
  Table,
  TableHeader,
  TableCell,
  TableFooter,
  TableContainer,
  Input,
  Card,
  CardBody,
  Pagination,
} from '@windmill/react-ui';

import useAsync from '../hooks/useAsync.js';
import useFilter from '../../../../../../../Users/husey/Downloads/Compressed/dashtar-admin-frontend-master/dashtar-admin-frontend-master/src/hooks/useFilter.js';
import NotFound from '../../../../../../../Users/husey/Downloads/Compressed/dashtar-admin-frontend-master/dashtar-admin-frontend-master/src/components/table/NotFound.js';
import UserServices from '../../../../../../../Users/husey/Downloads/Compressed/dashtar-admin-frontend-master/dashtar-admin-frontend-master/src/services/UserServices.js';
import Loading from '../../../../../../../Users/husey/Downloads/Compressed/dashtar-admin-frontend-master/dashtar-admin-frontend-master/src/components/preloader/Loading.js';
import PageTitle from '../../../../../../../Users/husey/Downloads/Compressed/dashtar-admin-frontend-master/dashtar-admin-frontend-master/src/components/Typography/PageTitle.js';
import CustomerTable from '../../../../../../../Users/husey/Downloads/Compressed/dashtar-admin-frontend-master/dashtar-admin-frontend-master/src/components/customer/CustomerTable.js';

const Customers = () => {
  const { data, loading } = useAsync(UserServices.getAllUsers);

  const {
    userRef,
    handleChangePage,
    totalResults,
    resultsPerPage,
    dataTable,
    serviceData,
    handleSubmitUser,
  } = useFilter(data);

  return (
    <>
      <PageTitle>Customers</PageTitle>
      <Card className="min-w-0 shadow-xs overflow-hidden bg-white dark:bg-gray-800 mb-5">
        <CardBody>
          <form
            onSubmit={handleSubmitUser}
            className="py-3 grid gap-4 lg:gap-6 xl:gap-6 md:flex xl:flex"
          >
            <div className="flex-grow-0 md:flex-grow lg:flex-grow xl:flex-grow">
              <Input
                ref={userRef}
                className="border h-12 text-sm focus:outline-none block w-full bg-gray-100 border-transparent focus:bg-white"
                type="search"
                name="search"
                placeholder="Search by name/email/phone"
              />
              <button
                type="submit"
                className="absolute right-0 top-0 mt-5 mr-1"
              ></button>
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
                <TableCell>Joining Date</TableCell>
                <TableCell>Name</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Phone</TableCell>
                <TableCell className="text-right">Actions</TableCell>
              </tr>
            </TableHeader>
            <CustomerTable customers={dataTable} />
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
        <NotFound title="Customer" />
      )}
    </>
  );
};

export default Customers;
