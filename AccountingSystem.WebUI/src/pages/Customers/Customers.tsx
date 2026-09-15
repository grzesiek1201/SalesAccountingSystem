import { FormEvent, useEffect, useMemo, useState } from 'react'
import { getApiErrorMessage } from '../../api/axiosClient'
import { Button } from '../../components/Button/Button'
import { FormField } from '../../components/Form/FormField'
import { StatusView } from '../../components/State/StatusView'
import { DataTable } from '../../components/Table/DataTable'
import { customerService } from '../../services/customerService'
import type { Customer, CustomerPayload } from '../../models/Customer'

const emptyForm: CustomerPayload = {
  name: '',
  nip: '',
  email: '',
  street: '',
  city: '',
  zipCode: ''
}

export function Customers() {
  const [customers, setCustomers] = useState<Customer[]>([])
  const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(null)
  const [editingCustomer, setEditingCustomer] = useState<Customer | null>(null)
  const [form, setForm] = useState<CustomerPayload>(emptyForm)
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function loadCustomers() {
    setLoading(true)
    setError(null)

    try {
      setCustomers(await customerService.getAll())
    } catch (err) {
      setError(getApiErrorMessage(err))
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    void loadCustomers()
  }, [])

  const filteredCustomers = useMemo(() => {
    const normalizedSearch = search.trim().toLowerCase()

    return customers
      .filter((customer) => {
        if (!normalizedSearch) {
          return true
        }

        return [customer.name, customer.nip, customer.email, customer.city]
          .join(' ')
          .toLowerCase()
          .includes(normalizedSearch)
      })
      .sort((left, right) => left.name.localeCompare(right.name, 'pl'))
  }, [customers, search])

  function startCreate() {
    setEditingCustomer(null)
    setSelectedCustomer(null)
    setForm(emptyForm)
  }

  function startEdit(customer: Customer) {
    setEditingCustomer(customer)
    setSelectedCustomer(customer)
    setForm({
      name: customer.name,
      nip: customer.nip,
      email: customer.email,
      street: customer.street,
      city: customer.city,
      zipCode: customer.zipCode
    })
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSaving(true)
    setError(null)

    try {
      if (editingCustomer) {
        const updated = await customerService.update(editingCustomer.id, form)
        setSelectedCustomer(updated)
        setEditingCustomer(updated)
      } else {
        await customerService.create(form)
        setForm(emptyForm)
      }

      await loadCustomers()
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
          <span className="eyebrow">Kartoteka</span>
          <h2>Klienci</h2>
        </div>
        <Button onClick={startCreate}>Dodaj klienta</Button>
      </div>

      <div className="toolbar">
        <input
          className="input"
          placeholder="Szukaj po nazwie, NIP, emailu lub mieście"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
        />
        <Button variant="secondary" onClick={loadCustomers} disabled={loading}>
          Odśwież
        </Button>
      </div>

      <div className="split-view">
        <section className="panel">
          <StatusView
            loading={loading}
            error={error}
            isEmpty={customers.length === 0}
            loadingText="Ładowanie klientów..."
            emptyText="Brak klientów"
          />

          {!loading && !error && customers.length > 0 && (
            <DataTable
              data={filteredCustomers}
              getRowKey={(customer) => customer.id}
              emptyText="Brak klientów dla podanego filtra"
              columns={[
                {
                  header: 'Nazwa',
                  render: (customer) => (
                    <button
                      className="link-button"
                      onClick={() => setSelectedCustomer(customer)}
                    >
                      {customer.name}
                    </button>
                  )
                },
                { header: 'NIP', render: (customer) => customer.nip || '-' },
                { header: 'Email', render: (customer) => customer.email || '-' },
                { header: 'Miasto', render: (customer) => customer.city || '-' },
                {
                  header: 'Akcje',
                  render: (customer) => (
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => startEdit(customer)}
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
          <div className="section-heading">
            <h3>{editingCustomer ? 'Edycja klienta' : 'Dodaj klienta'}</h3>
          </div>

          <form className="form-grid" onSubmit={handleSubmit}>
            <FormField label="Nazwa">
              <input
                className="input"
                value={form.name}
                onChange={(event) =>
                  setForm((current) => ({ ...current, name: event.target.value }))
                }
                required
              />
            </FormField>
            <FormField label="NIP">
              <input
                className="input"
                value={form.nip}
                onChange={(event) =>
                  setForm((current) => ({ ...current, nip: event.target.value }))
                }
              />
            </FormField>
            <FormField label="Email">
              <input
                className="input"
                type="email"
                value={form.email}
                onChange={(event) =>
                  setForm((current) => ({ ...current, email: event.target.value }))
                }
              />
            </FormField>
            <FormField label="Ulica">
              <input
                className="input"
                value={form.street}
                onChange={(event) =>
                  setForm((current) => ({ ...current, street: event.target.value }))
                }
              />
            </FormField>
            <FormField label="Miasto">
              <input
                className="input"
                value={form.city}
                onChange={(event) =>
                  setForm((current) => ({ ...current, city: event.target.value }))
                }
              />
            </FormField>
            <FormField label="Kod pocztowy">
              <input
                className="input"
                value={form.zipCode}
                onChange={(event) =>
                  setForm((current) => ({
                    ...current,
                    zipCode: event.target.value
                  }))
                }
              />
            </FormField>

            <div className="button-row">
              <Button type="submit" disabled={saving}>
                {editingCustomer ? 'Zapisz zmiany' : 'Utwórz klienta'}
              </Button>
              <Button type="button" variant="secondary" onClick={startCreate}>
                Wyczyść
              </Button>
            </div>
          </form>

          {selectedCustomer && (
            <div className="details-box">
              <span className="eyebrow">Szczegóły klienta</span>
              <strong>{selectedCustomer.name}</strong>
              <p>
                {selectedCustomer.street || '-'}, {selectedCustomer.zipCode || '-'}{' '}
                {selectedCustomer.city || '-'}
              </p>
              <p>{selectedCustomer.email || 'Brak emaila'}</p>
            </div>
          )}
        </aside>
      </div>
    </section>
  )
}
