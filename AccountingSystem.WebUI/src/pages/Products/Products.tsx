import { FormEvent, useEffect, useMemo, useState } from 'react'
import { getApiErrorMessage } from '../../api/axiosClient'
import { StatusBadge } from '../../components/Badge/StatusBadge'
import { Button } from '../../components/Button/Button'
import { FormField } from '../../components/Form/FormField'
import { StatusView } from '../../components/State/StatusView'
import { DataTable } from '../../components/Table/DataTable'
import { productCategoryService } from '../../services/productCategoryService'
import { productService } from '../../services/productService'
import { formatCurrency } from '../../utils/formatters'
import type {
  Product,
  ProductCategory,
  ProductCategoryPayload,
  ProductPayload,
  ProductUnit
} from '../../models/Product'

const productUnits: ProductUnit[] = [
  'Piece',
  'Kilogram',
  'Liter',
  'Meter',
  'Hour',
  'Package'
]

const emptyProductForm: ProductPayload = {
  name: '',
  productCode: '',
  vatRate: 23,
  unit: 'Piece',
  price: 0,
  categoryId: 0
}

const emptyCategoryForm: ProductCategoryPayload = {
  name: ''
}

export function Products() {
  const [products, setProducts] = useState<Product[]>([])
  const [categories, setCategories] = useState<ProductCategory[]>([])
  const [activeTab, setActiveTab] = useState<'products' | 'categories'>(
    'products'
  )
  const [productForm, setProductForm] = useState<ProductPayload>(emptyProductForm)
  const [categoryForm, setCategoryForm] =
    useState<ProductCategoryPayload>(emptyCategoryForm)
  const [editingProduct, setEditingProduct] = useState<Product | null>(null)
  const [editingCategory, setEditingCategory] =
    useState<ProductCategory | null>(null)
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function loadProductsData() {
    setLoading(true)
    setError(null)

    try {
      const [nextProducts, nextCategories] = await Promise.all([
        productService.getAll(),
        productCategoryService.getAll()
      ])

      setProducts(nextProducts)
      setCategories(nextCategories)
      setProductForm((current) => ({
        ...current,
        categoryId: current.categoryId || nextCategories[0]?.id || 0
      }))
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    void loadProductsData()
  }, [])

  const categoryById = useMemo(
    () => new Map(categories.map((category) => [category.id, category])),
    [categories]
  )

  const filteredProducts = useMemo(() => {
    const normalizedSearch = search.trim().toLowerCase()

    return products
      .filter((product) => {
        if (!normalizedSearch) {
          return true
        }

        const categoryName = categoryById.get(product.categoryId)?.name || ''

        return [product.name, product.productCode, categoryName]
          .join(' ')
          .toLowerCase()
          .includes(normalizedSearch)
      })
      .sort((left, right) => left.name.localeCompare(right.name, 'pl'))
  }, [categoryById, products, search])

  function startNewProduct() {
    setEditingProduct(null)
    setProductForm({
      ...emptyProductForm,
      categoryId: categories[0]?.id || 0
    })
  }

  function startEditProduct(product: Product) {
    setEditingProduct(product)
    setProductForm({
      name: product.name,
      productCode: product.productCode,
      vatRate: product.vatRate,
      unit: product.unit,
      price: product.price,
      categoryId: product.categoryId
    })
    setActiveTab('products')
  }

  function startNewCategory() {
    setEditingCategory(null)
    setCategoryForm(emptyCategoryForm)
  }

  function startEditCategory(category: ProductCategory) {
    setEditingCategory(category)
    setCategoryForm({ name: category.name })
    setActiveTab('categories')
  }

  async function saveProduct(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      const payload = {
        ...productForm,
        vatRate: Number(productForm.vatRate),
        price: Number(productForm.price),
        categoryId: Number(productForm.categoryId)
      }

      if (editingProduct) {
        await productService.update(editingProduct.id, payload)
      } else {
        await productService.create(payload)
      }

      startNewProduct()
      await loadProductsData()
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setSaving(false)
    }
  }

  async function saveCategory(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      if (editingCategory) {
        await productCategoryService.update(editingCategory.id, categoryForm)
      } else {
        await productCategoryService.create(categoryForm)
      }

      startNewCategory()
      await loadProductsData()
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setSaving(false)
    }
  }

  return (
    <section className="page">
      <div className="page-header">
        <div>
          <span className="eyebrow">Magazyn</span>
          <h2>Produkty</h2>
        </div>
        <div className="button-row">
          <Button onClick={startNewProduct}>Dodaj produkt</Button>
          <Button variant="secondary" onClick={startNewCategory}>
            Dodaj kategorię
          </Button>
        </div>
      </div>

      <div className="tabs">
        <button
          className={activeTab === 'products' ? 'tab active' : 'tab'}
          onClick={() => setActiveTab('products')}
        >
          Lista produktów
        </button>
        <button
          className={activeTab === 'categories' ? 'tab active' : 'tab'}
          onClick={() => setActiveTab('categories')}
        >
          Kategorie
        </button>
      </div>

      <div className="toolbar">
        <input
          className="input"
          placeholder="Szukaj produktu, kodu lub kategorii"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
        />
        <Button
          variant="secondary"
          onClick={loadProductsData}
          disabled={loading}
        >
          Odśwież
        </Button>
      </div>

      <div className="split-view">
        <section className="panel">
          <StatusView
            loading={loading}
            error={error}
            isEmpty={activeTab === 'products' ? products.length === 0 : categories.length === 0}
            loadingText="Ładowanie produktów..."
            emptyText={activeTab === 'products' ? 'Brak produktów' : 'Brak kategorii'}
          />

          {!loading && !error && activeTab === 'products' && (
            <DataTable
              data={filteredProducts}
              getRowKey={(product) => product.id}
              emptyText="Brak produktów dla podanego filtra"
              columns={[
                { header: 'Nazwa', render: (product) => product.name },
                {
                  header: 'Kod',
                  render: (product) => product.productCode || '-'
                },
                {
                  header: 'Kategoria',
                  render: (product) =>
                    categoryById.get(product.categoryId)?.name || '-'
                },
                {
                  header: 'Status',
                  render: (product) => {
                    const category = categoryById.get(product.categoryId)

                    return (
                      <StatusBadge tone={category?.isActive ? 'success' : 'warning'}>
                        {category?.isActive ? 'Aktywny' : 'Nieaktywny'}
                      </StatusBadge>
                    )
                  }
                },
                { header: 'Cena', render: (product) => formatCurrency(product.price) },
                {
                  header: 'Akcje',
                  render: (product) => (
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => startEditProduct(product)}
                    >
                      Edytuj
                    </Button>
                  )
                }
              ]}
            />
          )}

          {!loading && !error && activeTab === 'categories' && (
            <DataTable
              data={categories}
              getRowKey={(category) => category.id}
              emptyText="Brak kategorii"
              columns={[
                { header: 'Nazwa', render: (category) => category.name },
                {
                  header: 'Status aktywności',
                  render: (category) => (
                    <StatusBadge tone={category.isActive ? 'success' : 'warning'}>
                      {category.isActive ? 'Aktywna' : 'Nieaktywna'}
                    </StatusBadge>
                  )
                },
                {
                  header: 'Akcje',
                  render: (category) => (
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => startEditCategory(category)}
                    >
                      Edytuj
                    </Button>
                  )
                }
              ]}
            />
          )}
        </section>

        <aside className="panel side-panel">
          {activeTab === 'products' ? (
            <>
              <div className="section-heading">
                <h3>{editingProduct ? 'Edycja produktu' : 'Dodaj produkt'}</h3>
              </div>
              <form className="form-grid" onSubmit={saveProduct}>
                <FormField label="Nazwa">
                  <input
                    className="input"
                    value={productForm.name}
                    onChange={(event) =>
                      setProductForm((current) => ({
                        ...current,
                        name: event.target.value
                      }))
                    }
                    required
                  />
                </FormField>
                <FormField label="Kod produktu">
                  <input
                    className="input"
                    value={productForm.productCode}
                    onChange={(event) =>
                      setProductForm((current) => ({
                        ...current,
                        productCode: event.target.value
                      }))
                    }
                    required
                  />
                </FormField>
                <FormField label="Kategoria">
                  <select
                    className="input"
                    value={productForm.categoryId}
                    onChange={(event) =>
                      setProductForm((current) => ({
                        ...current,
                        categoryId: Number(event.target.value)
                      }))
                    }
                    required
                  >
                    <option value={0}>Wybierz kategorię</option>
                    {categories.map((category) => (
                      <option key={category.id} value={category.id}>
                        {category.name}
                      </option>
                    ))}
                  </select>
                </FormField>
                <FormField label="Jednostka">
                  <select
                    className="input"
                    value={productForm.unit}
                    onChange={(event) =>
                      setProductForm((current) => ({
                        ...current,
                        unit: event.target.value as ProductUnit
                      }))
                    }
                  >
                    {productUnits.map((unit) => (
                      <option key={unit} value={unit}>
                        {unit}
                      </option>
                    ))}
                  </select>
                </FormField>
                <FormField label="Cena netto">
                  <input
                    className="input"
                    type="number"
                    min="0"
                    step="0.01"
                    value={productForm.price}
                    onChange={(event) =>
                      setProductForm((current) => ({
                        ...current,
                        price: Number(event.target.value)
                      }))
                    }
                  />
                </FormField>
                <FormField label="VAT">
                  <input
                    className="input"
                    type="number"
                    min="0"
                    step="1"
                    value={productForm.vatRate}
                    onChange={(event) =>
                      setProductForm((current) => ({
                        ...current,
                        vatRate: Number(event.target.value)
                      }))
                    }
                  />
                </FormField>
                <div className="button-row">
                  <Button
                    type="submit"
                    disabled={saving || productForm.categoryId === 0}
                  >
                    {editingProduct ? 'Zapisz zmiany' : 'Utwórz produkt'}
                  </Button>
                  <Button type="button" variant="secondary" onClick={startNewProduct}>
                    Wyczyść
                  </Button>
                </div>
              </form>
            </>
          ) : (
            <>
              <div className="section-heading">
                <h3>
                  {editingCategory ? 'Edycja kategorii' : 'Dodaj kategorię'}
                </h3>
              </div>
              <form className="form-grid" onSubmit={saveCategory}>
                <FormField label="Nazwa">
                  <input
                    className="input"
                    value={categoryForm.name}
                    onChange={(event) =>
                      setCategoryForm({ name: event.target.value })
                    }
                    required
                  />
                </FormField>
                <div className="button-row">
                  <Button type="submit" disabled={saving}>
                    {editingCategory ? 'Zapisz zmiany' : 'Utwórz kategorię'}
                  </Button>
                  <Button
                    type="button"
                    variant="secondary"
                    onClick={startNewCategory}
                  >
                    Wyczyść
                  </Button>
                </div>
              </form>
            </>
          )}
        </aside>
      </div>
    </section>
  )
}
