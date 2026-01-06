// src/components/Table/View/DataTableView.tsx
import React from 'react';
import type { ColumnDef, SortDir } from './types';
import DataTableHeader from './Toolbar/DataTableHeader';
import DataTableFilters from './Toolbar/DataTableFilters';
import DataTableHead from './View/DataTableHead';
import DataTableBody from './View/DataTableBody';
import DataTablePagination from './View/DataTablePagination';
import LoadingState from './_components/LoadingState';
import ErrorState from './_components/ErrorState';

/**
 * DataTableView için prop tanımları.
 * Bu arayüz, tablonun tüm davranışlarını ve görünümünü kontrol eden propları içerir.
 * @template T Tablodaki her bir satır verisinin tipini belirtir.
 */
interface DataTableViewProps<T> {
    // --- Veri ve Sütun Propları ---

    /**
     * @description Tabloda gösterilecek olan veri satırları dizisi.
     * @usage DataTableBody bileşenine aktarılır ve satırları render etmek için kullanılır.
     */
    rows: T[];
    /**
     * @description Tabloda o an GÖRÜNÜR olan sütunların tanımını içeren dizi.
     * @usage DataTableHead (başlıklar için) ve DataTableBody (hücreler için) bileşenlerine aktarılır.
     */
    columns: ColumnDef<T>[];
    /**
     * @description Tablo için tanımlanmış TÜM sütunların dizisi. Görünürlük ayarları için kullanılır.
     * @usage DataTableHeader içerisindeki sütun görünürlüğü menüsünü doldurmak için kullanılır.
     */
    allColumns: ColumnDef<T>[];

    // --- Durum Propları (State Props) ---

    /**
     * @description Veri yüklenirken `true` olur ve yükleme animasyonunu gösterir.
     * @usage Yükleme durumunu göstermek için `LoadingState` bileşenini render eder.
     */
    loading: boolean;
    /**
     * @description Veri yüklenirken bir hata oluşursa hata mesajını içerir.
     * @usage Hata durumunu göstermek için `ErrorState` bileşenini render eder.
     */
    error?: string;

    // --- Sayfalama Propları (Pagination Props) ---

    /**
     * @description Aktif olan sayfa numarası (genellikle 1'den başlar).
     * @usage DataTablePagination bileşenine mevcut sayfayı belirtmek için aktarılır.
     */
    page: number;
    /**
     * @description Sayfa başına gösterilecek satır sayısı.
     * @usage DataTablePagination ve DataTableHeader (sayfa boyutu seçimi) bileşenlerine aktarılır.
     */
    pageSize: number;
    /**
     * @description Veri setindeki toplam satır sayısı (sayfalamadan bağımsız).
     * @usage DataTablePagination bileşenine toplam sayfa sayısını hesaplaması için aktarılır.
     */
    total: number;
    /**
     * @description Sayfa numarası değiştiğinde tetiklenen callback fonksiyonu.
     * @usage DataTablePagination bileşenindeki sayfa değiştirme butonlarına bağlanır.
     */
    onPageChange: (page: number) => void;
    /**
     * @description Sayfa boyutu (satır sayısı) değiştiğinde tetiklenen callback fonksiyonu.
     * @usage DataTablePagination ve DataTableHeader bileşenlerindeki sayfa boyutu seçicisine bağlanır.
     */
    onPageSizeChange: (size: number) => void;

    // --- Sıralama Propları (Sorting Props) ---

    /**
     * @description Hangi sütuna göre sıralama yapıldığını belirten sütun ID'si. `null` ise sıralama yok.
     * @usage DataTableHead bileşenine hangi sütunun aktif sıralama göstergesine sahip olacağını belirtmek için aktarılır.
     */
    sortBy: string | null; // bu gidecek
    /**
     * @description Sıralama yönü ('asc' - artan, 'desc' - azalan).
     * @usage DataTableHead bileşenine sıralama okunun yönünü belirtmek için aktarılır.
     */
    sortDir: SortDir;
    /**
     * @description Bir sütun başlığına tıklandığında sıralamayı değiştiren callback fonksiyonu.
     * @usage DataTableHead bileşenindeki sütun başlıklarına tıklama olayı olarak bağlanır.
     */
    onToggleSort: (col: ColumnDef<T>) => void;

    // --- Satır Seçimi Propları (Row Selection Props) ---

    /**
     * @description `true` ise satırların başında seçim kutucukları gösterilir.
     * @usage DataTableHead ve DataTableBody bileşenlerine seçim kutucuklarını render edip etmeyeceklerini bildirir.
     */
    selectableRows: boolean;
    /**
     * @description Seçili olan satırların `key`'lerini içeren bir Set.
     * @usage DataTableBody bileşenine hangi satırların seçili olduğunu belirtmek için aktarılır.
     */
    selected: Set<React.Key>;
    /**
     * @description `selected` state'ini güncelleyen fonksiyon.
     * @usage DataTableBody bileşenindeki satır seçim kutucuklarına bağlanır.
     */
    setSelected: React.Dispatch<React.SetStateAction<Set<React.Key>>>;
    /**
     * @description Başlıktaki "tümünü seç" kutucuğu işaretlendiğinde tetiklenen callback.
     * @usage DataTableHead bileşenindeki ana seçim kutucuğuna bağlanır.
     */
    onSelectAll: (checked: boolean) => void;
    /**
     * @description Mevcut sayfadaki tüm satırların seçili olup olmadığını belirtir.
     * @usage DataTableHead bileşenindeki ana seçim kutucuğunun durumunu (işaretli) belirler.
     */
    allSelected: boolean;
    /**
     * @description Mevcut sayfadaki bazı satırların (ama hepsinin değil) seçili olup olmadığını belirtir.
     * @usage DataTableHead bileşenindeki ana seçim kutucuğunun durumunu (belirsiz/indeterminate) belirler.
     */
    someSelected: boolean;

    // --- Başlık ve Araç Çubuğu Propları (Header & Toolbar Props) ---

    /**
     * @description Tablonun ana başlığı.
     * @usage DataTableHeader bileşeninde gösterilir.
     */
    title?: string;
    /**
     * @description Başlığın altında yer alan açıklama metni.
     * @usage DataTableHeader bileşeninde gösterilir.
     */
    description?: string;
    /**
     * @description Veriyi dışa aktarma (export) işlemi tetiklendiğinde çalışan callback.
     * @usage DataTableHeader içerisindeki "Export" butonuna bağlanır.
     */
    onExport: (format: 'csv' | 'xlsx' | 'pdf') => void;
    /**
     * @description Yazdırma işlemi tetiklendiğinde çalışan callback.
     * @usage DataTableHeader içerisindeki "Print" butonuna bağlanır.
     */
    onPrint: () => void;
    /**
     * @description Hangi sütunun görünür olduğunu tutan bir obje (`{ columnId: boolean }`).
     * @usage DataTableHeader bileşenindeki sütun görünürlüğü menüsünün durumunu belirler.
     */
    visibleColumns: Record<string, boolean>;
    /**
     * @description Sütun görünürlüğü değiştirildiğinde tetiklenen callback.
     * @usage DataTableHeader içerisindeki sütun görünürlüğü menüsüne bağlanır.
     */
    onVisibleColumnsChange: (columns: Record<string, boolean>) => void;
    /**
     * @description Ayarlar menüsüne eklenecek özel React bileşeni.
     * @usage DataTableHeader içerisindeki ayarlar dropdown'una eklenir.
     */
    settingsDropdownContent?: React.ReactNode;

    // --- Filtreleme ve Arama Propları (Filtering & Search Props) ---

    /**
     * @description Arama kutusunun mevcut değeri.
     * @usage DataTableFilters bileşenindeki arama input'unun değerini kontrol eder.
     */
    searchValue: string;
    /**
     * @description Arama kutusunun değeri değiştiğinde tetiklenen callback.
     * @usage DataTableFilters bileşenindeki arama input'una bağlanır.
     */
    onSearchChange: (value: string) => void;
    /**
     * @description Arama kutusu için placeholder metni.
     * @usage DataTableFilters bileşenindeki arama input'una aktarılır.
     */
    searchPlaceholder?: string;
    /**
     * @description Aktif filtrelerin değerlerini tutan obje (`{ columnId: value }`).
     * @usage DataTableFilters bileşenine hangi filtrelerin aktif olduğunu belirtmek için aktarılır.
     */
    filters: Record<string, any>;
    /**
     * @description Bir filtre değeri değiştiğinde tetiklenen callback.
     * @usage DataTableFilters içerisindeki filtre elemanlarına bağlanır.
     */
    onFilterChange: (columnId: string, value: any) => void;
    /**
     * @description Tüm filtreleri ve aramayı temizleyen callback.
     * @usage DataTableFilters içerisindeki "Temizle" butonuna bağlanır.
     */
    onClearAll: () => void;
    /**
     * @description Veriyi manuel olarak yeniden yüklemeyi tetikleyen callback.
     * @usage DataTableFilters içerisindeki "Yenile" butonuna bağlanır.
     */
    onRefresh: () => void;
    /**
     * @description `true` ise "Temizle" ve "Yenile" butonları gösterilir.
     * @usage DataTableFilters bileşenine bu butonları render edip etmeyeceğini bildirir.
     */
    showFilterButtons: boolean;
    /**
     * @description Filtre alanına eklenecek özel React bileşeni.
     * @usage DataTableFilters bileşeni içerisine yerleştirilir.
     */
    customFiltersContent?: React.ReactNode;

    // --- Stil ve Görünüm Propları (Styling & Appearance Props) ---

    /**
     * @description Tablonun genel görsel stili ('default', 'modern', 'minimal').
     * @usage DataTableHead ve DataTableBody bileşenlerine stil varyantını iletir.
     */
    variant: 'default' | 'modern' | 'minimal';
    /**
     * @description Satırların yoğunluğu/yüksekliği ('compact', 'normal', 'comfortable').
     * @usage DataTableHeader (ayarlar menüsü), DataTableHead ve DataTableBody'e aktarılır.
     */
    density: 'compact' | 'normal' | 'comfortable';
    /**
     * @description Tablo boş olduğunda gösterilecek başlık.
     * @usage DataTableBody bileşenine, veri olmadığında gösterilecek metin için aktarılır.
     */
    emptyStateTitle?: string;
    /**
     * @description Tablo boş olduğunda gösterilecek açıklama metni.
     * @usage DataTableBody bileşenine, veri olmadığında gösterilecek metin için aktarılır.
     */
    emptyStateDescription?: string;

    // --- Tıklama Olayları (Click Events) ---

    /**
     * @description Hücre tıklama olayının (`onCellClick`) sadece bu ID'ye sahip sütunda aktif olmasını sağlar.
     * @usage DataTableBody bileşenine hangi sütunun hücrelerinin tıklanabilir olacağını bildirir.
     */
    clickColumnId?: string;
    /**
     * @description `clickColumnId` ile belirtilen sütundaki bir hücreye tıklandığında tetiklenen callback.
     * @usage DataTableBody bileşenine aktarılır.
     */
    onCellClick?: (row: T) => void;
}

function DataTableView<T>(props: DataTableViewProps<T>) {
    // Propları doğrudan kullanıyoruz, burada yeni fonksiyon tanımlamıyoruz.
    const {
        // Veri ve Sütunlar
        rows, columns, allColumns,
        // Durumlar
        loading, error,
        // Sayfalama
        page, pageSize, total, onPageChange, onPageSizeChange,
        // Sıralama
        sortBy, sortDir, onToggleSort,
        // Satır Seçimi
        selectableRows, selected, setSelected, onSelectAll, allSelected, someSelected,
        // Başlık ve Araçlar
        title, description, onExport, onPrint, visibleColumns, onVisibleColumnsChange, settingsDropdownContent,
        // Filtreleme ve Arama
        searchValue, onSearchChange, searchPlaceholder, filters, onFilterChange, onClearAll, onRefresh, showFilterButtons, customFiltersContent,
        // Stil ve Görünüm
        variant, density, emptyStateTitle, emptyStateDescription,
        // Tıklama Olayları
        clickColumnId, onCellClick
    } = props;

    return (
        <div className="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
            {/* Tablonun başlığını, açıklamasını ve export/print/ayarlar gibi kontrolleri içerir */}
            <DataTableHeader
                title={title}
                description={description}
                onExport={onExport}
                onPrint={onPrint}
                columns={allColumns} // Sütun görünürlüğü için tüm sütunları gönder
                visibleColumns={visibleColumns}
                onVisibleColumnsChange={onVisibleColumnsChange}
                pageSize={pageSize}
                onPageSizeChange={onPageSizeChange}
                density={density}
                customSettingsContent={settingsDropdownContent}
            />

            {/* Arama, filtreler, temizle ve yenile butonlarını içerir */}
            <DataTableFilters
                searchValue={searchValue}
                onSearchChange={onSearchChange}
                searchPlaceholder={searchPlaceholder}
                columns={columns}
                filters={filters}
                onFilterChange={onFilterChange}
                onClearAll={onClearAll}
                showFilterButtons={showFilterButtons}
                onRefresh={onRefresh}
                loading={loading}
                customFiltersContent={customFiltersContent}
            />

            <div className="overflow-auto relative">
                <table className="w-full table-fixed md:table-auto">
                    {/* Tablonun başlık (thead) kısmını render eder */}
                    <DataTableHead
                        columns={columns}
                        selectableRows={selectableRows}
                        sortBy={sortBy}
                        sortDir={sortDir}
                        onToggleSort={onToggleSort}
                        variant={variant}
                        density={density}
                        onSelectAll={onSelectAll}
                        allSelected={allSelected}
                        someSelected={someSelected}
                    />

                    {/* Yükleme, hata veya veri durumuna göre tablo gövdesini (tbody) render eder */}
                    {loading ? (
                        <tbody><tr><td colSpan={columns.length + (selectableRows ? 1 : 0)}><LoadingState /></td></tr></tbody>
                    ) : error ? (
                        <tbody><tr><td colSpan={columns.length + (selectableRows ? 1 : 0)}><ErrorState error={error} /></td></tr></tbody>
                    ) : (
                        <DataTableBody
                            rows={rows}
                            columns={columns}
                            selectableRows={selectableRows}
                            selected={selected}
                            setSelected={setSelected}
                            clickColumnId={clickColumnId}
                            onCellClick={onCellClick}
                            variant={variant}
                            density={density}
                            emptyStateTitle={emptyStateTitle}
                            emptyStateDescription={emptyStateDescription}
                        />
                    )}
                </table>
            </div>

            {/* Yükleme veya hata durumu yoksa ve veri varsa sayfalama kontrollerini gösterir */}
            {!loading && !error && total > 0 && (
                <DataTablePagination
                    page={page}
                    pageSize={pageSize}
                    total={total}
                    setPage={onPageChange}
                    setPageSize={onPageSizeChange}
                />
            )}
        </div>
    );
}

export default React.memo(DataTableView) as typeof DataTableView;